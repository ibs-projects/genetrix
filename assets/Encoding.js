function encoding(string) {
    //&,=,+,/

    var _s = string + "";

    var r1 = /&/gi;//_v1
    //var r2 = /\=/gi;//_v2
    //var r3 = /\+/gi;//_v3
    //var r4 = /\//gi;//_v4

    var r5 = /</gi;//_v5
    var r6 = />/gi;//_v6
    _s = _s.replace(r5, "_v5");
    _s = _s.replace(r6, "_v6");
    _s = _s.replace(r1, "_v1");
    //_s = _s.replace(r2, "_v2");
    //_s = _s.replace(r3, "_v3");
    //_s = _s.replace(r4, "_v4");

    return _s;
}

function decoding(_s)
{  
    try {
        var r1 = /_v1/gi;
        //var r2 = "_v2";
        //var r3 = "_v3";
        //var r4 = "_v4";

        var r5 = /_v5/gi;
        var r6 =/_v6/gi;
        _s = _s.replace(r5, "<");
        _s = _s.replace(r6, ">");

        _s = _s.replace(r1, "&");
        //_s = _s.replace(r2, "=");
        //_s = _s.replace(r3, "+");
        //_s = _s.replace(r4, "/");
    } catch (e) {
        //
    }
    alert(_s);
    return _s;
}