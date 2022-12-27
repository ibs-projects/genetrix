try {
    function getFilesize(fSize) {
        var fSExt = new Array('Bytes', 'KB', 'MB', 'GB');
        i = 0; while (fSize > 900) { fSize /= 1024; i++; }
        return (Math.round(fSize * 100) / 100) + ' ' + fSExt[i];
    }

} catch (e) {

}


