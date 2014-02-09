$(function () {

    $(".info_copy_button").each(function () {
        //Create a new clipboard client
        var clip = new ZeroClipboard.Client();
        ZeroClipboard.setMoviePath('../../content/ZeroClipboard10.swf');

        clip.setHandCursor(true);

        clip.addEventListener('load', function (client) {
            debugstr("Flash movie loaded and ready.");
        });

        //Cache the last td and the parent row    
        var lastTd = $(this)[0];

        //Glue the clipboard client to the last td in each row
        clip.glue(lastTd);

        //Grab the text from the parent row of the icon
        var txt = $(this).find("input").val();
        //alert(txt);
        clip.setText(txt);

        clip.addEventListener('mouseover', function (client) {
            $(this).find("a").addClass("hover");
        });

        //Add a complete event to let the user know the text was copied
        clip.addEventListener('complete', function (client, text) {
            debugstr("Copied text to clipboard:\n" + text);
        });
    });
});

function debugstr(msg) {
    var p = document.createElement('p');
    p.innerHTML = msg;
    $('#d_debug').append(p);
}

function copy_to_clipboard(text) {
    if (window.clipboardData) {
        window.clipboardData.setData('text', text);
    }
    else {
        var clipboarddiv = document.getElementById('divclipboardswf');
        if (clipboarddiv == null) {
            clipboarddiv = document.createElement('div');
            clipboarddiv.setAttribute("name", "divclipboardswf");
            clipboarddiv.setAttribute("id", "divclipboardswf");
            document.body.appendChild(clipboarddiv);
        }
        clipboarddiv.innerHTML = '<embed src="../../content/clipboard.swf" FlashVars="clipboard=' +
                                     encodeURIComponent(text) +
                                     '" width="0" height="0" type="application/x-shockwave-flash"></embed>';
    }

    alert('The text is copied to your clipboard...');
    return false;
}
