using netly.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;

namespace netly.Controllers
{
    public class BaseController : Controller
    {
        [DllImport("netapi32.dll", CharSet = CharSet.Auto)]
        static extern int NetWkstaGetInfo(string server,
            int level,
            out IntPtr pBuf);

        [DllImport("netapi32.dll")]
        static extern int NetApiBufferFree(IntPtr pBuf);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        class WKSTA_INFO_100
        {
            public int wki100_platform_id;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_computername;
            [MarshalAs(UnmanagedType.LPWStr)]
            public string wki100_langroup;
            public int wki100_ver_major;
            public int wki100_ver_minor;
        }

        private string _machineName = null;
        public virtual string MachineName
        {
            get
            {
                WKSTA_INFO_100 info;
                IntPtr pBuffer = IntPtr.Zero;
                string _machineName;

                try
                {
                    int retval = NetWkstaGetInfo(null, 100, out pBuffer);
                    if (retval == 0)
                    {
                        info = (WKSTA_INFO_100)Marshal.PtrToStructure(pBuffer, typeof(WKSTA_INFO_100));
                        _machineName = info.wki100_computername;
                    }
                    else
                    {
                        _machineName = "00";
                    }

                    return _machineName;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (pBuffer != IntPtr.Zero)
                        NetApiBufferFree(pBuffer);
                }
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Set the machine name so we can display in Site.master
            ViewData["_machineName"] = System.Text.RegularExpressions.Regex.Replace(this.MachineName, @"[^\d]", "");

            // Set the assembly version so we can display in Site.master
            ViewData["_assemblyVersion"] = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        protected WebClient GetWebClient()
        {
            WebClient client = new WebClient();

            if (this.ProxyConfig.IsEnabled)
            {
                client.Proxy = new WebProxy(this.ProxyConfig.ServerName, this.ProxyConfig.Port);

                if (!string.IsNullOrEmpty(this.ProxyConfig.Username))
                {
                    var cred = new NetworkCredential(this.ProxyConfig.Username, this.ProxyConfig.Password);
                    client.Proxy.Credentials = cred;
                }
            }

            return client;
        }

        private ProxyServerConfigSettings _proxyConfig = null;
        public ProxyServerConfigSettings ProxyConfig
        {
            get
            {
                if (_proxyConfig == null)
                {
                    _proxyConfig = new ProxyServerConfigSettings()
                    {
                        IsEnabled = Utils.GetConfigValueAsBool("Proxy.Enabled", false),
                        ServerName = Utils.GetConfigValueAsString("Proxy.ServerName"),
                        Port = Utils.GetConfigValueAsInt("Proxy.Port"),
                        Username = Utils.GetConfigValueAsString("Proxy.Username"),
                        Password = Utils.GetConfigValueAsString("Proxy.Password")
                    };
                }

                return _proxyConfig;
            }
        }
    }

    public class ProxyServerConfigSettings
    {
        public bool IsEnabled { get; set; }
        public string ServerName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}