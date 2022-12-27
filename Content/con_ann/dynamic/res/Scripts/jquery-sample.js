var Enum_Demos = {
    'helloworld': 'demo01',
    'useevent': 'demo02',
    'sourcelist': 'demo03',
    'withoutui': 'demo04',
    'autofeeder': 'demo05',
    'scanandsave': 'demo16',
    'customscan': 'demo06',
    'loadlocal': 'demo13',
    'saveimage': 'demo14',
    'httpupload': 'demo07',
    'sendextrainfo': 'demo17',
    'httpdownload': 'demo12',
    'showeditor': 'demo10',
    'customedit': 'demo11',
    'navigation': 'demo08',
    'thumbnails': 'demo09',
    'rasterizer': 'demo19',
    'serversideocr': 'demo20',
    'clientsideocr': 'demo23',
    'scanbarcode': 'demo21',
	'scanwebcam': 'demo22',
	'cameradbr': 'demo24'
},
    aryDemo = {
        categories:
            [
                {
                    name: "Get Started", type: "core", demos:
                        [
                            {
                                name: "Hello World",
                                API: [
                                    {
                                        name: "SelectSource()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#SelectSource"
                                    },
                                    {
                                        name: "OpenSource()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OpenSource"
                                    },
                                    {
                                        name: "AcquireImage()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#AcquireImage"
                                    }],
                                desc: "Hello World of Dynamic Web TWAIN", link: "Samples/Getting Started/HelloWorld.html", className: Enum_Demos.helloworld, screenshotLink: ""
                            },
                            {
                                name: "Use Event",
                                API: [
                                    {
                                        name: "OnPostAllTransfers", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostAllTransfers"
                                    },
                                    {
                                        name: "Width", APILink: "https://www.dynamsoft.com/docs/dwt/API/Display-UI.html#Width"
                                    },
                                    {
                                        name: "Height", APILink: "https://www.dynamsoft.com/docs/dwt/API/Display-UI.html#Height"
                                    }],
                                desc: "Register built-in Dynamic Web TWAIN Events", link: "Samples/Getting Started/UseEvent.html", className: Enum_Demos.useevent, screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "Scan", type: "core", demos:
                        [
                            {
                                name: 'Scan from source list',
                                API: [
                                    {
                                        name: "AcquireImage()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#AcquireImage"
                                    },
                                    {
                                        name: "SelectSourceByIndex()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#SelectSourceByIndex"
                                    }],
                                desc: "Scan images from a source in the drop down menu", link: "Samples/Scan/SourceList.html", className: Enum_Demos.sourcelist, screenshotLink: ""
                            },
                            {
                                name: 'Scan without UI',
                                API: [
                                    {
                                        name: "IfShowUI", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#IfShowUI"
                                    }],
                                desc: "Scan images with or without scanner UI", link: "Samples/Scan/ScanWithoutUI.html", className: Enum_Demos.withoutui, screenshotLink: ""
                            },
                            {

                                name: 'Scan with autofeeder',
                                API: [
                                    {
                                        name: "IfFeederEnabled", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#IfFeederEnabled"
                                    }],
                                desc: "Scan images with or without feeder enabled", link: "Samples/Scan/AutoFeeder.html", className: Enum_Demos.autofeeder, screenshotLink: ""
                            },
                            {
                                name: 'Scan and Save',
                                API: [
                                    {
                                        name: "OnPostAllTransfers", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostAllTransfers"
                                    },
                                    {
                                        name: "OnPostTransfer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostTransfer"
                                    },
                                    {
                                        name: "SaveAsJPEG()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAsJPEG"
                                    },
                                    {
                                        name: "SaveAllAsMultiPageTIFF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAllAsMultiPageTIFF"
                                    },
                                    {
                                        name: "SaveAllAsPDF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAllAsPDF"
                                    }],
                                desc: "Scan image(s) and save them automatically", link: "Samples/Load Save/SaveImages.html", className: Enum_Demos.scanandsave, screenshotLink: ""
                            },
                            {
                                name: 'Custom Scan',
                                API: [
                                    {
                                        name: "Resolution", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#Resolution"
                                    },
                                    {
                                        name: "PixelType", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#PixelType"
                                    },
                                    {
                                        name: "IfShowUI", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#IfShowUI"
                                    },
                                    {
                                        name: "IfFeederEnabled", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#IfFeederEnabled"
                                    }],
                                desc: "Scan image with customized resolution and pixel type", link: "Samples/Scan/CustomScan.html", className: Enum_Demos.customscan, screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "Load Save", type: "core", demos:
                        [
                            {
                                name: 'Load local',
                                API: [
                                    {
                                        name: "LoadImageEx()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#LoadImageEx"
                                    }],
                                desc: "Load image(s) from local drive", link: "Samples/Load Save/SaveImages.html", className: Enum_Demos.loadlocal, screenshotLink: ""
                            },
                            {
                                name: 'Save image',
                                API: [
                                    {
                                        name: "OnPostAllTransfers", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostAllTransfers"
                                    },
                                    {
                                        name: "OnPostTransfer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostTransfer"
                                    },
                                    {
                                        name: "SaveAsJPEG()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAsJPEG"
                                    },
                                    {
                                        name: "SaveAllAsMultiPageTIFF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAllAsMultiPageTIFF"
                                    },
                                    {
                                        name: "SaveAllAsPDF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#SaveAllAsPDF"
                                    }],
                                desc: "Scan or load image(s) and save them automatically", link: "Samples/Load Save/SaveImages.html", className: Enum_Demos.saveimage, screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "Upload Download", type: "core", demos:
                        [
                            {
                                name: 'HTTP Upload',
                                API: [
                                    {
                                        name: "HTTPUploadThroughPost()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#HTTPUploadThroughPost"
                                    },
                                    {
                                        name: "HTTPUploadAllThroughPostAsMultiPageTIFF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#HTTPUploadAllThroughPostAsMultiPageTIFF"
                                    },
                                    {
                                        name: "HTTPUploadAllThroughPostAsPDF()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#HTTPUploadAllThroughPostAsPDF"
                                    }],
                                desc: "Upload images to the server", link: "Samples/Upload Download/Visual Studio Demo/UploadWithHTTP.html", className: Enum_Demos.httpupload, screenshotLink: "res/Images/UploadWithHTTP.png"
                            },
                            {
                                name: 'Send extra info',
                                API: [
                                    {
                                        name: "ClearAllHTTPFormField()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#ClearAllHTTPFormField"
                                    },
                                    {
                                        name: "SetHTTPFormField()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#SetHTTPFormField"
                                    }],
                                desc: "Send Extra info with the images to the server", link: "Samples/Upload Download/Visual Studio Demo/SendExtraInfo.html", className: Enum_Demos.sendextrainfo, screenshotLink: "res/Images/SendExtraInfo.png"
                            },
                            {
                                name: 'HTTP Download',
                                API: [{
                                    name: "HTTPDownload()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Upload-Download.html#HTTPDownload"
                                }],
                                desc: "Download images from the server", link: "Samples/Upload Download/Visual Studio Demo/DownloadWithHTTP.html", className: Enum_Demos.httpdownload, screenshotLink: "res/Images/DownloadWithHTTP.png"
                            }
                        ]
                },
                {
                    name: "Edit", type: "core", demos:
                        [
                            {
                                name: 'Show editor',
                                API: [
                                    {
                                        name: "ShowImageEditor()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#ShowImageEditor"
                                    }],
                                desc: "Show Dynamic Web TWAIN's built-in image editor", link: "Samples/Edit/ShowEditor.html", className: Enum_Demos.showeditor, screenshotLink: ""
                            },
                            {
                                name: 'Custom edit',
                                API: [
                                    {
                                        name: "RotateLeft()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#RotateLeft"
                                    },
                                    {
                                        name: "RotateRight()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#RotateRight"
                                    },
                                    {
                                        name: "Mirror()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#Mirror"
                                    },
                                    {
                                        name: "Flip()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#Flip"
                                    },
                                    {
                                        name: "HowManyImagesInBuffer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Runtime-Info.html#HowManyImagesInBuffer"
                                    }],
                                desc: "Rotate, mirror or flip an image", link: "Samples/Edit/Edit.html", className: Enum_Demos.customedit, screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "Display", type: "core", demos:
                        [
                            {
                                name: 'Navigation',
                                API: [
                                    {
                                        name: "SetViewMode()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Display-UI.html#SetViewMode"
                                    },
                                    {
                                        name: "OnMouseClick", APILink: "https://www.dynamsoft.com/docs/dwt/API/Display-UI.html#OnMouseClick"
                                    },
                                    {
                                        name: "CurrentImageIndexInBuffer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Runtime-Info.html#CurrentImageIndexInBuffer"
                                    },
                                    {
                                        name: "HowManyImagesInBuffer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Runtime-Info.html#HowManyImagesInBuffer"
                                    }],
                                desc: "Navigate images with custom preview mode", link: "Samples/Display/Navigation.html", className: Enum_Demos.navigation, screenshotLink: ""
                            },
                            {
                                name: 'Thumbnail',
                                API: [
                                    {
                                        name: "CopyToClipboard()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Edit.html#CopyToClipboard"
                                    },
                                    {
                                        name: "LoadDibFromClipboard()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#LoadDibFromClipboard"
                                    },
                                    {
                                        name: "OnPostTransfer", APILink: "https://www.dynamsoft.com/docs/dwt/API/Basic-Scan.html#OnPostTransfer"
                                    },
                                    {
                                        name: "OnPostLoad", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#OnPostLoad"
                                    },
                                    {
                                        name: "OnMouseClick", APILink: "https://www.dynamsoft.com/docs/dwt/API/Display-UI.html#OnMouseClick"
                                    }],
                                desc: "Thumbnails sample with two controls", link: "Samples/Thumbnail/Thumbnail.html", className: Enum_Demos.thumbnails, screenshotLink: ""
                            }
                        ]
                },
				{
                    name: "Pure-JS-Solution", type: "addon", demos:
                        [
                            {
                                name: 'Pure-JS-Solution',
                                bExternalLink: false,
                                API: "",
                                desc: "Pure JS Solution",
                                link: "Samples/Pure-JS-Solution/PureJSSolution.html",
                                className: Enum_Demos.cameradbr,
                                screenshotLink: "res/Images/PureJsSolution.png"
                            }
                        ]
                },
                {
                    name: "PDF Rasterizer", type: "addon", demos:
                        [
                            {
                                name: 'PDF Rasterizer',
                                bExternalLink: false,
                                API: [
                                    {
                                        name: "Addon.PDF.Download()", APILink: "https://www.dynamsoft.com/docs/dwt/API/PDFR.html#Download"
                                    },
                                    {
                                        name: "Addon.PDF.SetResolution()", APILink: "https://www.dynamsoft.com/docs/dwt/API/PDFR.html#SetResolution"
                                    },
                                    {
                                        name: "Addon.PDF.SetConvertMode()", APILink: "https://www.dynamsoft.com/docs/dwt/API/PDFR.html#SetConvertMode"
                                    },
                                    {
                                        name: "LoadImageEx()", APILink: "https://www.dynamsoft.com/docs/dwt/API/Load-Save.html#LoadImageEx"
                                    }],
                                desc: "PDF Rasterizer", link: "Samples/PDFRasterizer/PDFRasterizer.html", className: Enum_Demos.rasterizer, screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "OCRB", type: "addon", demos:
                        [
                            {
                                name: 'Client-side OCR',
                                bExternalLink: false,
                                API: "",
                                desc: "Scan and Do client-side OCR",
                                link: "Samples/OCRBasic/OCRBasicClientSide.html",
                                className: Enum_Demos.clientsideocr,
                                screenshotLink: ""
                            }
                        ]
                },
                {
                    name: "Scan+Barcode", type: "addon", demos:
                        [
                            {
                                name: 'Scan + Barcode',
                                //bExternalLink: true,
                                API: "",
                                desc: "Scan and read barcode",
                                link: "Samples/Scan+Barcode/online_demo_scan_Barcode.html",
                                className: Enum_Demos.scanbarcode,
                                screenshotLink: ""
                            }
                        ]
                },
				 {
                    name: "Scan+Webcam", type: "addon", demos:
                        [
                            {
                                name: 'Scan + Webcam',
                                //bExternalLink: true,
                                API: "",
                                desc: "Acquire with scanners and cameras",
                                link: "Samples/Scan+Webcam/online_demo_scan_Webcam.html",
                                className: Enum_Demos.scanwebcam,
                                screenshotLink: ""
                            }
                        ]
                }
            ]
    };

function loadDemos() {
    $("#goback").live("click", function () {
        $(".demo a")[0].click();
    });
    $("#demoCat").empty();
    $("#demoAddonCat").empty();
	Dynamsoft.Lib.asyncGetNavInfo().then(function(_navInfo){
		for (var i = 0; i < aryDemo.categories.length; i++) {
			if (aryDemo.categories[i].demos != null) {
				if (aryDemo.categories[i].type == "addon") {
						for (var j = 0; j < aryDemo.categories[i].demos.length; j++) {
							var o = aryDemo.categories[i].demos[j];	
							if(o.className == Enum_Demos.serversideocr){
								$("#demoAddonCat").append("<li class='demo'><a class='" + o.className + "'>" + o.name + "</a></li>");
							} else if((_navInfo.bEdge && parseInt(_navInfo.strBrowserVersion) >= 80) || ( _navInfo.bEdge && (o.className == Enum_Demos.clientsideocr || o.className == Enum_Demos.scanbarcode))){
								 $("#demoAddonCat").append("<li class='demo'><a href='" + o.link + "' target='_blank'>" + o.name + "</a></li>");
							} else {			
								if (o.bExternalLink) {
									$("#demoAddonCat").append("<li class='demo'><a href='" + o.link + "' target='_blank'>" + o.name + "</a></li>");
								}
								else {
									$("#demoAddonCat").append("<li class='demo'><a class='" + o.className + "'>" + o.name + "</a></li>");
								}
							}
						}
				}
				else {
					var strList = "";
					for (var g = 0; g < aryDemo.categories[i].demos.length; g++) {
						var tempClassName = aryDemo.categories[i].demos[g].className;
						if(_navInfo.bEdge && parseInt(_navInfo.strBrowserVersion) >= 80 && tempClassName != Enum_Demos.httpupload && tempClassName != Enum_Demos.sendextrainfo && tempClassName != Enum_Demos.httpdownload)
							strList += "<li class='demo'><a href='" + aryDemo.categories[i].demos[g].link + "' target='_blank'>" + aryDemo.categories[i].demos[g].name + "</a></li>";
						else	
							strList += "<li class='demo'><a class='" + aryDemo.categories[i].demos[g].className + "'>" + aryDemo.categories[i].demos[g].name + "</a></li>";
					}	
					if (i == 0) {
						$("#demoCat").append("<li class='liCat expand'><span>" + aryDemo.categories[i].name + "<i class='fa fa-angle-up'></i></span><ul class='demoList'>" + strList + "</ul></li>");
					}
					else {
						$("#demoCat").append("<li class='liCat'><span>" + aryDemo.categories[i].name + "<i class='fa fa-angle-down'></i></span><ul class='demoList'>" + strList + "</ul></li>");
					}
				}
			}
		}
	});
}

// collapse demos
$("#demoCat li.liCat span").live("click", function () {
    var o = $(this);
    o.parent(".liCat").toggleClass("expand");
    o = o.children();
    if (o.hasClass('fa-angle-up')) {
        o.removeClass('fa-angle-up');
        o.addClass('fa-angle-down');
    } else {
        o.removeClass('fa-angle-down');
        o.addClass('fa-angle-up');
    }
});


$(".demo a").live("click", function () {
    var currentDemo = $(this).attr("class");
    $(".catList li").removeClass("CurrentDemo");
    $(this).closest("li").addClass("CurrentDemo");
    $(".demoCode").hide();
    var strPath = window.location.href;
    strPath = strPath.substring(0, strPath.lastIndexOf("/") + 1);
    var _href = '';
    for (var i = 0; i < aryDemo.categories.length; i++) {
        for (var j = 0; j < aryDemo.categories[i].demos.length; j++) {
            if (aryDemo.categories[i].demos[j].className == currentDemo) {
                $("#demoDesc").html(aryDemo.categories[i].demos[j].desc);
                if (currentDemo == Enum_Demos.scanbarcode || currentDemo == Enum_Demos.scanwebcam || currentDemo == Enum_Demos.clientsideocr) {
                    var _width = '1039px', demoMargin = '0px', _height = '900px';
                    if (currentDemo == Enum_Demos.clientsideocr)
                        _height = '920px';
                    if (currentDemo == Enum_Demos.scanwebcam) {
                        _width = '1121px';
                        demoMargin = '0 0 0 -40px';
                    }
                    $("#goback").show();
                    $("#demoDesc").parent().css('display', 'none');
                    $('#sidebar').css('display', 'none');
                    $('#sampleContent').css({
                        'width': '1039px',
                        'margin-left': '0px',
                        'overflow': 'hidden'
                    });
                    $('.demo-main').css({
                        'width': _width,
                        'margin': demoMargin,
                        'border': '0px',
                        'padding': '0px',
                        'overflow': 'hidden'
                    });
                    $("#frmDemo").css({
                        "margin-top": '-5px',
                        "width": $("#frmDemo").parent().css("width"),
                        "height": _height
                    });
                    $("#Samples").css({
                        'width': '1039px',
                        'margin': '0'
                    });
                    $(".description").css({
                        'margin-left': '50px'
                    });
                } else {
                    $("#goback").hide();
                    $("#demoDesc").parent().css('display', '');
                    $('#sidebar').css('display', 'block');
                    $('#sampleContent').css({
                        'width': '715px',
                        'margin-left': '47px'
                    });
                    $('.demo-main').css({
                        'width': '700px',
                        'margin': '20px 0',
                        'border': '1px solid #ddd',
                        'padding': '5px'
                    });
					if(currentDemo == Enum_Demos.cameradbr){
						 $("#frmDemo").css({
                        "width": parseInt($("#frmDemo").parent().css("width")) - 10 + "px",
                        "height": '600px'
                    });
					} else {
						$("#frmDemo").css({
							"width": parseInt($("#frmDemo").parent().css("width")) - 10 + "px",
							"height": '500px'
						});
					}
                    $("#Samples").css({
                        'width': '940px',
                        'margin': '0 0 0 35px'
                    });
                    $(".description").css({
                        'margin-left': '0px'
                    });
                }
                if (currentDemo == Enum_Demos.serversideocr || currentDemo == Enum_Demos.scanbarcode || currentDemo == Enum_Demos.scanwebcam || currentDemo == Enum_Demos.clientsideocr || currentDemo == Enum_Demos.mobilecamera) {
                    $('.description').hide();
                } else {
                    $('.description').show();
                }
                /******************If non-IE on Win, view source******************/
                ua = (navigator.userAgent.toLowerCase());
                if (ua.indexOf("msie") == -1 && ua.indexOf('trident') == -1)
                    _href = /*"view-source:" + */strPath;
                /*****************************************************************/
                if (aryDemo.categories[i].demos[j].link != "")
					$("#demoLink").html("The complete source code can be found at <a style='text-decoration: underline; font-weight: bold' href='" + _href + aryDemo.categories[i].demos[j].link + "' target='_blank'>" + strPath + aryDemo.categories[i].demos[j].link + "</a>");
                else
                    $("#demoLink").html("");
                if (aryDemo.categories[i].demos[j].screenshotLink == "") {
                    $("#frmDemo").attr("src", aryDemo.categories[i].demos[j].link);
                }
                else {
                    $("#frmDemo").attr("src", aryDemo.categories[i].demos[j].screenshotLink);
                }
                var strAPI = "";
                if (aryDemo.categories[i].demos[j].API != "") {
                    for (var k = 0; k < aryDemo.categories[i].demos[j].API.length; k++) {
                        if (strAPI != "") {
                            strAPI += ", ";
                        }
                        if (aryDemo.categories[i].demos[j].API[k].APILink != null) {
                            strAPI += "<a href='" + aryDemo.categories[i].demos[j].API[k].APILink + "' target='_blank'>" + aryDemo.categories[i].demos[j].API[k].name + "</a>";
                        }
                        else {
                            strAPI += aryDemo.categories[i].demos[j].API[k].name;
                        }
                    }
                    strAPI = "Main API(s) used in this sample: " + strAPI;
                }
                $("#spnDemoAPIName").html(strAPI);

            }
        }
    }

    $("." + currentDemo + "").show();
});

$("#viewSource").live("click", function () {
    $("#demoSource").toggle();

    var o = $(this).children();
    if (o.hasClass('fa-angle-up')) {
        o.removeClass('fa-angle-up');
        o.addClass('fa-angle-down');
    } else {
        o.removeClass('fa-angle-down');
        o.addClass('fa-angle-up');
    }
});
