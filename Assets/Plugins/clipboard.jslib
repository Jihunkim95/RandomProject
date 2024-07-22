mergeInto(LibraryManager.library, {
    CopyToClipboard: function (text) {
        var str = UTF8ToString(text);
        navigator.clipboard.writeText(str).then(function() {
            console.log('Text copied to clipboard');
        }).catch(function(error) {
            console.error('Failed to copy text: ', error);
        });
    },
    GetClipboardText: function() {
        navigator.clipboard.readText().then(function(text) {
            console.log("Clipboard text:", text);
            if (typeof Module.unityInstance !== 'undefined' && Module.unityInstance !== null) {
                Module.unityInstance.SendMessage("TextCopyHandler", "SetInputFieldText", text);
            } else {
                console.error('unityInstance is not defined or null');
            }
        }).catch(function(error) {
            console.error('Failed to read text: ', error);
        });
    },
    CheckUnityInstanceInitialized: function() {
        return typeof Module.unityInstance !== 'undefined' && Module.unityInstance !== null;
    }
});
