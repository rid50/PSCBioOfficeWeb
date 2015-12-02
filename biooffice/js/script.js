var lang = "en";
var divVideo;
var divWsq;

var numberOfFingerImages;

$(document).ready(function () {
	$.ajaxSetup({ cache: false, async: false });

	$.blockUI.defaults.message = '<img src="images/ajax-loader.gif"/>';
	$.blockUI.defaults.css.top = '3px';
	$.blockUI.defaults.css.left = '3px';
	$.blockUI.defaults.css.textAlign = 'left';
	$.blockUI.defaults.css.border = 'none';
	$.blockUI.defaults.css.color = 'transparent';
	$.blockUI.defaults.css.backgroundColor = 'transparent';
	$.blockUI.defaults.css.cursor = 'default';
	
	$.blockUI.defaults.overlayCSS.backgroundColor = '#383838';
	$.blockUI.defaults.overlayCSS.opacity = 0.2;
	
	if (navigator.userAgent.match(/msie/i))
		$.blockUI.defaults.overlayCSS.cursor = 'default';

	$.blockUI.defaults.centerX = true;
	$.blockUI.defaults.centerY = true;

	// Ajax activity indicator bound to ajax start/stop document events
	$(document)
		.ajaxStart(function(){
			$.blockUI();
		})
		.ajaxStop(function(){
			$.unblockUI();
		});
/*
	$.get("get_ini.php")
		.done(function(data) {
			idp = data.IdP;
			idpSource = data.IdPSource;
			documentSource = data.documentSource;
			lang = data.lang;
			searchInterval = data.searchInterval;
		});
*/	

	$("#flagKuwait").off("click").on("click", function(event){
		if ($("body[dir='ltr']").length) {
			lang = 'ar';
			//$(videoControl).hide();
			toggleLanguage('ar', 'rtl');
			//$(videoControl).show();
		}
	});

	$("#flagUK").off("click").on("click", function(event){
		if ($("body[dir='rtl']").length) {
			lang = 'en';
			toggleLanguage('en', 'ltr');
		}
	});

	if (lang == "ar") {
		toggleLanguage('ar', 'rtl');
	} else {
		jQuery.i18n.properties({
			name:'Messages', 
			path:'bundle/', 
			mode:'both',
			language: 'en'
		});	
	}

	$('button').button();
	
	$('.wsq, .wsq_to_scan').on('load', function (e) {
		//if ($('#div_wsq').css('display') == 'none')
		//$('#pre_scanned_fingers').slideDown(500);
		if (--numberOfFingerImages == 0)
			$.unblockUI();
	})
	.on('error', function (e) {
		//if ($('#div_wsq').css('display') == 'none')
		//$('#pre_scanned_fingers').slideDown(500);
        //$('#error_box').val("ERROR!!!!");	
	    $.get("ImageHandler/ImageHandler.ashx?getSessionState=")
		    .done(function(data) {
            //alert(data);
              $('#error_box').val(data);
		    });

		if (--numberOfFingerImages == 0) {
       	    $.get("ImageHandler/ImageHandler.ashx?getSessionState=&clearError=");
			$.unblockUI();
        }
	});
	
	$(function() {
		var pictureTab = $('#pictureTab'), fingerTab = $('#fingerTab');
		$("#accordion").accordion({
			//active: false,
			collapsible: false,
			beforeActivate: function( event, ui ) {
				//try {
					$('#error_box').val("");		
					switch (ui.newHeader.index()) {
						case 0:
                            //cleanImageTags();

							fingerTab = $('#fingerTab>div').detach();
							fingerTab.appendTo('#pictureTab');
							
                            $('#divVideo').show();
                            $('#divWsq').hide();
							if (videoControl !== undefined) {
								videoControl.StartAxVideoControl();
								$('#divPhoto').hide();
								$('#TakeImage').show();
							} else {
								$('#divPhoto').show();
								$('#TakeImage').hide();
							}
/*
							//if (!$(videoControl).is(':visible')) {
								if (divWsq == null) {
									$('#divWsq').hide();
									divWsq = $('#divWsq').detach();
								}
								if (divVideo) {
									divVideo.appendTo('#left-section');
									divVideo.show();
									divVideo = null;
									if (videoControl !== undefined)
										videoControl.StartAxVideoControl();
								//	$(videoControl).show();
								}
								//$('#pre_scanned_fingers').show();
							//}
*/
							break;
						case 2:
                            //cleanImageTags();

							pictureTab = $('#pictureTab>div').detach();
							pictureTab.appendTo('#fingerTab');
							$('#TakeImage').show();

                            $('#divWsq').show();
                            $('#divVideo').hide();
							$('#divPhoto').hide();
							if (videoControl !== undefined)
								videoControl.StopAxVideoControl();

/*
							if (divVideo == null) {
								if (videoControl !== undefined)
									videoControl.StopAxVideoControl();
									
								$('#divVideo').hide();
								divVideo = $('#divVideo').detach();
							}
								
								//$('#pre_scanned_fingers').hide();
								
							if (divWsq === undefined)
								divWsq = $('#divWsq');
								
							if (divWsq) {
								divWsq.appendTo('#left-section');
								$('#divWsq').show();
								divWsq = null;
							}

								//$(videoControl).hide();
							//}
*/
							break;
					}

                   	setTimeout(function() {
       				    $('#GetImage').triggerHandler( "click" );
	                }, 100 );

				//}catch(e) {}
			}
		});
		
//		$("#accordion").bind("keydown", function (event) {
//			//var keycode = (event.keyCode ? event.keyCode : (event.which ? event.which : event.charCode));
//			var keycode = event.keyCode || event.which;

//			if (keycode == 13) {
//				//event.stopImmediatePropagation();
//				//event.preventDefault();
//				//$('#searchButton').click();
//				$('#getPicture').triggerHandler( "click" );
//				//document.getElementById(btnFocus).click();
//			}
//        });

	});

	$("#LeftHand, #RightHand, #Thumbs, #TakeImage").on("click", function(event){
        if ($('#divVideo').is(':visible'))
            return;

        var id = $('#user_id').val();
        if (id == "") {
            alert("Please enter a valid ID");
            return;
        }

		var buttonId = this.id;
        var chBoxesArr = [];
		var chBoxesToScan;
        if (this.id == "TakeImage") {
		   $("#fingers_to_scan input[type='checkbox']").prop('checked', true);
           chBoxesArr.push("1");
        } else {
    	    chBoxesToScan = $(this).parent().siblings().children('input');
		    $.each(chBoxesToScan, function(index, value) {
			    chBoxesArr.push(value.checked ? '1' : '0');
		    });
		}

		//var data = "{" + JSON.stringify(chBoxesArr) + "}";
		//var data = "{\"checkBoxesStates\":" + JSON.stringify(chBoxesArr) + "}";
		var data = "checkBoxesStates=" + chBoxesArr.join();
		//data = null;
		
		$('#error_box').val("");	
        // Check if PSCWindowsService service was started
		var url = "http://biooffice:8080/PSCWindowsService/CommandService/CheckConnection/";

		$.blockUI();
		
		var xmlHttp = new XMLHttpRequest();
		try {
			xmlHttp.open( "GET", url, false );
			xmlHttp.send();
			//alert("Status: " + xmlHttp.status);
			if (xmlHttp.status != 200) {
				$.unblockUI();
				$('#error_box').val("Error connecting to PSCWindowsService: " + xmlHttp.status);
				return;
			}
		} catch (e) {
			$.unblockUI();
			$('#error_box').val("Check if PSCWindowsService service is running");
			return;	
		}
	
		url = "http://biooffice:8080/PSCWindowsService/CommandService/" + buttonId + "/" + id;
	
		$.ajax({ 
			type: "get",
			url: url,
			cache: false,
			//crossDomain: true,
			//async: false,
			//contentType: "application/json; charset=utf-8",
			//contentType: "text/json; charset=utf-8",
			//contentType: "application/x-www-form-urlencoded; charset=UTF-8",

			//contentType: "application/json; charset=utf-8",
			//contentType: "application/xml; charset=utf-8",
			//dataType: "xml",
			dataType: "jsonp",
			data: data,
		})
		.done(function( data ) {
			/*
			var object = JSON.parse(data.d);
			if (object.Error == '') {
				//Alert the persons name
				alert(object.Response);
			}
			*/
			if (data.result == "error") {
				$('#error_box').val(data.message);
				$.unblockUI();
			} else {
				//$('#error_box').val(data.result);

                var result = data.result.split('');
                //chBoxesArr.forEach(function(o, index) {
                //    if (chBoxesArr[index] == '1')
                //        if (chBoxesArr[index] | result[index])
                            
                //});
				
                var user_id = $('#user_id').val();
                var urlImg = 'ImageHandler/ImageHandler.ashx';
                var rand = Math.random();
				if (chBoxesToScan == undefined)
					chBoxesToScan = $("#fingers_to_scan input[type='checkbox']");
				
				var imgTagsToRead = [];
                $.each(chBoxesToScan, function(index, value) {
                    if (value.checked) {
                        var imgTag = $(value).parent().next().children();                        
                        if (result[index] == '1') {
				            imgTag.prop('src', '');
                        } else {
                            //$(".wsq_to_scan input[type='checkbox']").prop('checked', false);
                            $(value).prop('checked', false);
							imgTagsToRead.push(imgTag);
				            //imgTag.prop('src', urlImg + '?wsq=' + imgTag.prop('id') + '&id=' + user_id + '&rand=' + rand);
                        }
                    }
		        });

				if (imgTagsToRead.length != 0) {
					numberOfFingerImages = imgTagsToRead.length;
					imgTagsToRead.forEach(function(imgTag) {
						imgTag.prop('src', urlImg + '?wsq=' + imgTag.prop('id') + '&id=' + user_id + '&rand=' + rand);
					});
				} else
					$.unblockUI();
                //$.get('ImageHandler/ImageHandler.ashx?clearSession=');
            }

			//alert(data.Status);
			//alert(eval('data.' + buttonId + 'Result'));
			//var i = 0;
		})
		.fail(function(jqXHR, textStatus, errorThrown) {
			$.unblockUI();
			alert(errorThrown);
			//alert(jQuery.parseJSON(jqXHR.responseText));
		});
		//.always(function(jqXHR, textStatus, errorThrown) {
		//	alert(errorThrown);		
		//});	
	})		
	
	$('#left-section').html('<input type="text" id="error_box" readonly />');

	$('#divVideo').appendTo('#left-section');
	$('#divPhoto').appendTo('#left-section');
	$('#divWsq').appendTo('#left-section');
	$('#divVideo').show();
//return;
	try {
		//$('#divVideo').appendTo('#left-section');
		divVideo = null;
		videoControl.StartAxVideoControl();
		//$(videoControl).show();
	} catch(e) { videoControl = undefined;}
	
	if (videoControl === undefined) {
        $($('#accordion>span')[0]).text("Photo");
	    $('#divPhoto').show();
		$('#TakeImage').hide();
    }

    $.get('ImageHandler/ImageHandler.ashx?clearSession=')
   		.done(function(data) {
            if (data != "")
                $('#error_box').val(jQuery.parseJSON(data));
        })
    	.fail(function(data) {
            //$('#error_box').val(jQuery.parseJSON(data));
			//alert("fail: " + data.toString());
		});
});

function toggleLanguage(lang, dir) {
	var left = $(".floatLeft");
	var right = $(".floatRight");
	var direction = false; 
	$(left).toggleClass("floatLeft", direction);
	$(left).toggleClass("floatRight", !direction);
	$(right).toggleClass("floatRight", direction);
	$(right).toggleClass("floatLeft", !direction);
	$('body').attr('dir', dir);
	$('html').attr('lang', lang);

	jQuery.i18n.properties({
		name:'Messages', 
		path:'bundle/', 
		mode:'both',
		language: lang,
		callback: function() {
		
			$('#copyright').text(jQuery.i18n.prop('copyright'));

			if (dir == 'ltr') {
				//$("#left-section").css("margin", "0 6px 0 0");
				$("#left-section").css("margin-left", "0");
				$("#left-section").css("margin-right", "6px");
				$("#right-section").css("text-align", "right");
				//$("#accordion>span").css("text-align", "right");
				$("#left-section, #right-section").css("box-shadow", "4px 4px 2px #999");
				$(".ui-accordion .ui-accordion-content").css({'padding': '1em 8px 1em 0px'});
			} else {
				//$("#left-section").css("margin", "0, 0, 0, 6px");
				$("#left-section").css("margin-left", "6px");
				$("#left-section").css("margin-right", "0");
				$("#right-section").css("text-align", "left");
				//$("#accordion>span").css("text-align", "right");				
				$("#left-section, #right-section").css("box-shadow", "-4px 4px 2px #999");
				$(".ui-accordion .ui-accordion-content").css({'padding': '1em 0px 1em 8px'});
			}
						
			var obj = $("#accordion>span:nth-child(1)").contents().filter(function() {return this.nodeType == 3;});
			obj.get()[0].textContent = $.i18n.prop('video');
			$("#accordion>div>div>div:first>span").text($.i18n.prop('id'));
			$("#TakeImage").button({ label: $.i18n.prop('takeImage')});
			$("#TakeImage").attr({title: $.i18n.prop('takeImageTitle')});

			$("#GetImage").button({ label: $.i18n.prop('getImage')});
			$("#GetImage").attr({title: $.i18n.prop('getImageTitle')});
			
			$("#LeftHand, #RightHand, #Thumbs").button({ label: $.i18n.prop('start')});
			$("#LeftHand").attr({title: $.i18n.prop('scanLeftFingers')});
			$("#RightHand").attr({title: $.i18n.prop('scanRightFingers')});
			$("#Thumbs").attr({title: $.i18n.prop('scanThumbs')});

			obj = $("#accordion>span:nth-child(3)").contents().filter(function() {return this.nodeType == 3;});
			obj.get()[0].textContent = $.i18n.prop('fingerScan');
		}				
	});
}
/*
function startVideo() {
	if (videoControl !== undefined)
		videoControl.StartAxVideoControl();
	//document.videoControl.Background = "#e8e8e8";
	//videoControl.MyTitle = form1.txt.value;
	//document.videoControl.Open();
}

function stopVideo() {
	if (videoControl !== undefined)
		videoControl.StopAxVideoControl();
	//document.videoControl.Background = "#e8e8e8";
	//videoControl.MyTitle = form1.txt.value;
	//document.videoControl.Open();
}
*/

//function cleanImageTags() {
//	var imageClass;
//	if ($("#accordion").accordion( "option", "active" ) == 0)
//		imageClass = $('.wsq');
//	else
//		imageClass = $('.wsq_to_scan');
//		
//	(function() {
//		imageClass.each(function () {
//            $(this).removeAttr('src');
////                $('#divWsq img[id="lr"').removeAttr('src');
//		});		
//	})();
//}

$(function() {

	//$("#TakeImage").button({ label: $.i18n.prop('takeImage')});
	//$("#GetImage").button({ label: $.i18n.prop('getImage')});

//	var numberOfFingerImages = 10;

	$("#GetImage").on("click", function(event){
		//$.blockUI();
   		$('#error_box').val("");	
		getIt();
		//$.unblockUI();
	})
	
	$("#TakeImage").on("click", function(){
		//$.blockUI();
        $('#error_box').val("");	
		takeIt();
		//$.unblockUI();
	});

	function getIt() {
   		var id = $('#user_id').val();

        if (id == "") {
            $('#error_box').val("Enter a valid User Id");
            return;
        }

        var ret;
		if (videoControl !== undefined && $('#divVideo').is(':visible')) {
			ret = videoControl.GetIt(id);
            if (ret != "")
                $('#error_box').val(ret);
        } else {
            var user_id = $('#user_id').val();
            var urlImg = 'ImageHandler/ImageHandler.ashx';
            var rand = Math.random();
			$('#photo').prop('src', urlImg + '?id=' + user_id + '&rand=' + rand);
        }

		$.blockUI();

		var rand = Math.random();
		var urlImg = 'ImageHandler/ImageHandler.ashx';
		
		var imageClass;
		if ($("#accordion").accordion( "option", "active" ) == 0)
			imageClass = $('.wsq');
		else
			imageClass = $('.wsq_to_scan');
		
		numberOfFingerImages = imageClass.length;

		(function() {
			imageClass.each(function () {
				$(this).prop('src', urlImg + '?wsq=' + $(this).prop('id') + '&id=' + id + '&rand=' + rand);
			});		
		})();
/*
		var i = 0;
		var h = setInterval(function () {
			if (i < 10) {
				(function() {
					imageClass.each(function () {
						$(this).prop('src', urlImg + '?wsq=' + $(this).prop('id') + '&id=' + id + '&rand=' + rand);
					});		
				})();

				$('#user_id').val(id);
				id++;
				i++;
			}
			else {
				//clearInterval(h);
				id = "20095417";
				i = 0;
			}	
		}, 5000);
*/
	}

	function takeIt() {
        if ($('#user_id').val() == "") {
            $('#error_box').val("Enter a valid User Id");
            return;
        }

		if (videoControl !== undefined && $('#divVideo').is(':visible'))
			videoControl.TakeIt($('#user_id').val());
	}
});