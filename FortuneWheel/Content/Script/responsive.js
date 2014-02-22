$(function(){
    var UA = navigator.userAgent,
		win = window,
		width = 480,
		iw = win.innerWidth || width,
		ow = win.outerHeight || iw,
		sw = win.screen.width || iw,
		saw = win.screen.availWidth || iw,
		ih = win.innerHeight || width,
		oh = win.outerHeight || ih,
		ish = win.screen.height || ih,
		sah = win.screen.availHeight || ih,
		w = Math.min(iw, ow, sw, saw, ih, oh, ish, sah),
		ratio = w / width,
		dpr = win.devicePixelRatio;
    ratio = Math.min(ratio, dpr);
    if (ratio < 1) {
        var ctt = ',initial-scale=' + ratio + ',maximum-scale=' + ratio,
			metas = document.getElementsByTagName('meta'),
			us;
        if (/iphone|ipod/gi.test(UA)) {
            us = ',user-scalable=no';
        }
        if (/android/gi.test(UA)) {
            us = '';
        }
        ctt += us;
        for (var i = 0, meta; i < metas.length; i++) {
            meta = metas[i];
            if (meta.name == 'viewport') {
                meta.content += ctt;
            }
        }
    }
});