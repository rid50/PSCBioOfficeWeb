/*
	(function($){
		$.jgrid = {
			defaults : {
				//recordtext: "View!!!!!!! {0} - {1} of {2}",
				//emptyrecords: "No records to view",
				//loadtext: "Loading...",
				//pgtext : "Page#### {0} of {1}"
			},
		}
	})(jQuery);
*/

//$(document).ready(function () {
//    CheckupGrid.setupGrid($("#grid"), $("#pager"), $("#search"), (lang == 'en') ? 'ltr' : 'rtl');
//});
var myCustomSearch;

function toggleGrid(lang) {
	//$(".tagButton").text($.i18n.prop('Checkup'));
	var jgrid = $('#grid');
	jgrid.jqGrid('GridUnload');
	if ($.jgrid.hasOwnProperty("regional") && $.jgrid.regional.hasOwnProperty(lang))
		$.extend($.jgrid,$.jgrid.regional[lang]);

	CheckupGrid.setupGrid($("#grid"), $("#pager"), $("#grid_search_field"), (lang == 'en') ? 'ltr' : 'rtl');
	//CheckupGrid.setupGrid($("#grid"), $("#pager"), $("#mysearch"), (lang == 'en') ? 'ltr' : 'rtl');
}

CheckupGrid = {
    setupGrid: function (grid, pager, search, direction) {
        //        debugger; 
        grid.jqGrid({
			direction: direction,
            url: "json_db_crud_pdo.php",
			postData:{"func": "getCheckups"},
            mtype: "get",
            datatype: "json",
            colNames: [$.i18n.prop('FileNumber'), $.i18n.prop('CheckupNumber'), $.i18n.prop('Address'), $.i18n.prop('DateOfCheckup'), $.i18n.prop('Checker'), $.i18n.prop('CheckDate'), $.i18n.prop('Result'), $.i18n.prop('Checker')+'2', $.i18n.prop('CheckDate')+'2', $.i18n.prop('Result')+'2', $.i18n.prop('Checker')+'3', $.i18n.prop('CheckDate')+'3', $.i18n.prop('Result')+'3'],
            colModel: [ //http://php.net/manual/en/function.date.php
                        {name: 'file_no', index: 'file_no', align: 'left', width: '80px', sortable: true, resizable: true, frozen: true },
                        {name: 'form_no', index: 'form_no', align: 'right', width: '60px', sortable: true, editable: false, resizable: false },
                        {name: 'address', index: 'address', align: 'right', width: '190px', sortable: true, editable: false, resizable: true, searchoptions: { sopt: ['bw']} },
            //{ name: 'DateEntry', index: 'DateEntry', align: 'left', sortable: true, hidden: false, sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'M j Y h:i A', newformat: 'd-M-Y h:iA'} },  //DateEntry = "Dec 31 1999 12:00AM"
                        {name: 'date_ins', index: 'date_ins', align: 'center', width: '90px', sortable: true, hidden: false, resizable: false, sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'Y m d g:i:s', newformat: 'd-M-Y'} }, //DateEntry (src) = "12/31/1999 00:00:00"
                        {name: 'ch_name', index: 'ch_name', align: 'right', width: '80px', sortable: true, editable: false, resizable: false },
                        {name: 'check_1_dt', index: 'check_1_dt', align: 'center', width: '90px', sortable: true, hidden: false, resizable: false, sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'Y m d', newformat: 'd-M-Y'} }, //DateEntry (src) = "12/31/1999 00:00:00"
                        {name: 'result_1', index: 'result_1', align: 'right', width: '60px', sortable: true, editable: false, resizable: true },
                        {name: 'ch_name_2', index: 'ch_name_2', align: 'right', width: '80px', sortable: true, editable: false, resizable: false },
                        {name: 'check_2_dt', index: 'check_2_dt', align: 'center', width: '90px', sortable: true, hidden: false, resizable: false, sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'Y m d', newformat: 'd-M-Y'} }, //DateEntry (src) = "12/31/1999 00:00:00"
                        {name: 'result_2', index: 'result_2', align: 'right', width: '60px', sortable: false, editable: false, resizable: true },
                        {name: 'ch_name_3', index: 'ch_name_3', align: 'right', width: '80px', sortable: true, editable: false, resizable: false },
                        {name: 'check_3_dt', index: 'check_3_dt', align: 'center', width: '90px', sortable: true, hidden: false, resizable: false, sorttype: 'date', formatter: 'date', formatoptions: { srcformat: 'Y m d', newformat: 'd-M-Y'} }, //DateEntry (src) = "12/31/1999 00:00:00"
                        {name: 'result_3', index: 'result_3', align: 'right', width: '60px', sortable: false, editable: false, resizable: true },
                        //{name: 'ContractValue', index: 'contract_value', align: 'right', width: '45', sortable: false, hidden: false, editable: true },
                        //{name: 'Currency', index: 'currency', align: 'left', width: '15', sortable: false, hidden: false, editable: false },
                      ],
            rowNum: 15,
            rowList: [15, 30, 45],
			altclass: 'gridAltRows',
			altRows: true,
            loadui: "block",
			hidegrid: false,
            //multiboxonly: true,
            //multiselect: true,
            pager: pager,
            sortname: 'file_no',
            sortorder: "asc",
            viewrecords: true,
            //editurl: "Checkup/Edit",
            width: 682,
            height: '362px',
            //height: 'auto',
            /*rowheight: '30px',*/
            shrinkToFit: false,
            autowidth: false,
            rownumbers: true,
            caption: $.i18n.prop('Checkup'),
            toppager: false,
			/*
			jsonReader: {
				root: "Rows",
				page: "Page",
				total: "Total",
				records: "Records",
				repeatitems: false,
				userdata: "UserData",
				id: "Id"
			},
			*/
			gridComplete:  function() {
				//jQuery("#grid").jqGrid('setFrozenColumns');
			},
			afterInsertRow: function(rowid, rowdata, rowelem) {
				var i;
				var eresult;
				var ar = ['result_1', 'result_2', 'result_3'];
				for (var j in ar) {
					eresult = eval('rowelem.' + ar[j]);
					if (eresult != null) {
						i = 1;
						for (var key in result_enum) {
						//'result_enum'.some(function(key, index) {	//does not work
							if (i == parseInt(eresult)) {
								$(this).setCell(rowid, ar[j], jQuery.i18n.prop(key));
								//rowdata.result_1 = jQuery.i18n.prop(i);
								break;
							}
							i++;
						}
					}
				}
				
				//var i = 0;
				//i = 2;
				//$(this).setCell(id, "column_name", "<input type='hidden'>");
			},			
            loadError: function (xhr, st, err) {
                if (window.console) window.console.log('failed');
				alert ("Type: " + st + "; Response: " + xhr.status + " " + xhr.statusText);
                $('#alertContent').html("Type: " + st + "; Response: " + xhr.status + " " + xhr.statusText);
                //$('#alertContent').html(xhr.responseText);
                $('#alert').dialog('open');
                //$('#info').text(event.toString()).css({ 'color': 'orange', 'fontweight': 'bold' });
            },
            loadComplete: function (event) {
                if (event && event[0] && event[0].error != "") {
					if (window.console) window.console.log(event[0].error);
					alert (event[0].error);
				}
				
				//var ur = jQuery("#grid").getGridParam("url");
				//jQuery("#grid").setGridParam({ url: "json_db_crud_pdo.php", page: 1 });
				
				//$("#grid").jqGrid("setColProp", "file_no", { searchoptions: { } });

                //debugger;
                //jQuery('#grid_d').setGridParam({ url: "ContractDetails/List?contract_id=0", page: 1 }).trigger('reloadGrid');
                //$('.ui-pg-table').css({ 'width': '600px', 'width': 'auto', 'table-layout': 'auto', 'border': '0px solid green' });
                //$('#pager_center').css({ 'width': '500px', 'border': '0px solid red' });
                //$('#pager_d_center').css({ 'width': '500px', 'border': '0px solid red' });

                //$('#formContract .submit').attr('disabled', 'disabled');
                //$('#formContract')[0].reset();
                //$('#formContract').fadeOut(500);

                //jQuery('#grid_d').trigger("reloadGrid");
                //grid.jqGrid('setSelection', "1");
				//jQuery("#grid").jqGrid('setFrozenColumns');
				//jQuery("#grid").trigger("reloadGrid", [{ current: true}]);
				//$("#grid").jqGrid("destroyFrozenColumns")
				//	.jqGrid("setColProp", "file_no", { frozen: true })
				//	.jqGrid("setFrozenColumns")
				//	.trigger("reloadGrid", [{ current: true}]);
            },
            onSelectRow: function (ids) {
                //grid.jqGrid('setSelection', "1");
                //debugger;
				/*
                $('#formContract .submit').removeAttr("disabled");
                $('#formContract').fadeIn(500, 'linear', function () {
                    var v = $('#grid').getCell(ids, 'ContractId');
                    $('#contract_id').val($('#grid').getCell(ids, 'ContractId'));
                    $('#contract_name').val($('#grid').getCell(ids, 'ContractName'));
                    //var dt = $.format.date($('#grid').getCell(ids, 'DateEntry'), "dd/MM/yyyy");
                    //var dt = $.datepicker.formatDate('dd/mm/yy', new Date(2007, 1 - 1, 26));
                    var dtString = $('#grid').getCell(ids, 'DateEntry');
                    var dtDate = $.datepicker.parseDate('dd-M-yy', dtString);   //Dec 31 1999
                    dtString = $.datepicker.formatDate('dd/mm/yy', dtDate);     //31/12/1999
                    $('#date_entry').val(dtString);
                    //var v = $('#grid').getCell(ids, 'ContractValue');
                    //v = v.substring(0, v.indexOf(' '));
                    $('#contract_value').val($('#grid').getCell(ids, 'ContractValue'));
                    //$('#info').text(v);
                    //alert(v);
                });
                if (ids == null) {
                    ids = 0;
                    if (jQuery('#grid_d').getGridParam('records') > 0) {
                        jQuery('#grid_d').setGridParam({ url: "ContractDetails/List?contract_id=" + ids, page: 1 }).setCaption("Contract: " + ids).trigger('reloadGrid');
                    }
                } else {
                    //debugger;
                    //var rowData = grid.getRowData(ids);
                    //var contractId = rowData['ContractId'];
                    var contractId = $('#grid').getCell(ids, 'ContractId');
                    jQuery('#grid_d').setGridParam({ url: "ContractDetails/List?contract_id=" + contractId, page: 1 }).setCaption("Contract: " + contractId).trigger('reloadGrid');
                }
				
				*/
            }
            //        }).jqGrid('navGrid', pager, { edit: true, add: false, del: false, search: false, refresh: true });
        }).navGrid("#pager", { view: true, edit: false, add: false, del: false, search: true, refresh: true },
                            {}, // settings for edit
                            {}, // settings for add
                            {}, // settings for delete
							{
								closeOnEscape:true, 
								onClose: function(){
									delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchField'];
									delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchString'];
									delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchOper'];
								}
							}  // search options
                            //{sopt: ["cn"]} // Search options. Some options can be set on column level        
        );
/*				  
        myCustomSearch = search.filterGrid("#" + grid.attr("id"), {
            gridModel: false,
            filterModel: [{
                label: 'Search#',
                name: 'file_no',
                stype: 'text'
            }],
			searchButton: $.i18n.prop('Go'),
			clearButton: $.i18n.prop('Clear'),
			enableSearch: true,
			enableClear: true,
			
        })[0];
*/
		//jQuery("#grid").jqGrid('setFrozenColumns');

    }
};
/*
        $(document).ready(function () {
            MyGrid.ContractDetails.setupGrid($("#grid_d"), $("#pager_d"), $("#search_d"));
        });

        MyGrid.ContractDetails = {
            setupGrid: function (grid, pager, search) {
                grid.jqGrid({
                    //url: "ContractDetails/List?contract_id=AA/SCB/25/8/99",
                    url: "ContractDetails/List",
                    mtype: "get",
                    datatype: "json",
                    colNames: ['Supplier Id', 'Item Id', 'Part No', 'Unit Price', 'Cost Price', 'Currency'],
                    colModel: [
                        //{ name: 'ContractId', index: 'contract_id', width: 50, align: 'left', sortable: true, hidden: false },
                        { name: 'SupplierId', index: 'supplier_id', width: '70', sortable: true, hidden: false },
                        { name: 'ItemId', index: 'item_id', width: '35', align: 'right', sortable: true, hidden: false },
                        { name: 'PartNo', index: 'part_no', sortable: true, hidden: false },
                        { name: 'UnitPrice', index: 'unit_price', align: 'right', width: '50', sortable: true, hidden: false },
                        { name: 'CostPrice', index: 'cost_price', align: 'right', width: '50', sortable: true, hidden: false },
                        { name: 'Currency', index: 'currency', align: 'right', width: '40', sortable: true, hidden: false }
                      ],
                    rowNum: 10,
                    rowList: [10, 20],
                    pager: pager,
                    sortname: 'supplier_id, item_id',
                    sortorder: "asc",
                    viewrecords: true,
                    multiselect: false,
                    editurl: "ContractDetails/Edit",
                    width: '100%',
                    height: '100%',
                    shrinkToFit: true,
                    autowidth: true,
                    rownumbers: true,
                    caption: 'Contract Details',
                    loadError: function (xhr, st, err) {
                        if (window.console) window.console.log('failed');
                        $('#alertContent').html("Type: " + st + "; Response: " + xhr.status + " " + xhr.statusText);
                        //$('#alertContent').html(xhr.responseText);
                        $('#alert').dialog('open');
                        //$('#info').text(event.toString()).css({ 'color': 'orange', 'fontweight': 'bold' });
                    }
                }).navGrid("#pager_d", { edit: true, add: true, del: true, search: false });
                search.filterGrid("#" + grid.attr("id"), {
                    gridModel: false,
                    filterModel: [{
                        label: 'Search',
                        name: 'search_d',
                        stype: 'text'
                    }]
                });
            }
        };
*/
        var timeoutHnd = null;
        var flAuto = false;
        function doSearch(e) {
            if (timeoutHnd) {
                clearTimeout(timeoutHnd)
				timeoutHnd = null;
            }
			
            if (e.keyCode == 13) {
                timeoutHnd = setTimeout(gridReload, 500);
                return;
            }
            if (!flAuto) return;
            // var elem = ev.target||ev.srcElement;
            //if (timeoutHnd) {
            //    clearTimeout(timeoutHnd)
            //}
			
			//var keyCode = (e.keyCode ? e.keyCode : (e.which ? e.which : e.charCode));
			var keyCode = e.keyCode || e.which;

			// 65-90 	: A to Z
			// 8 		: Backspace
			// 46		: Delete
			// 48-57	: 0 to 9
			// 96-105	: 0 to 9 (Numpad)
            //if ((ev.keyCode >= 65 && ev.keyCode <= 90) || ev.keyCode == 8 || ev.keyCode == 46 || (ev.keyCode >= 48 && ev.keyCode <= 57) || (ev.keyCode >= 96 && ev.keyCode <= 105)) {
            if ((keyCode >= 65 && keyCode <= 90) || keyCode == 8 || keyCode == 46 || (keyCode >= 48 && keyCode <= 57) || (keyCode >= 96 && keyCode <= 105)) {
                timeoutHnd = setTimeout(gridReload, 500)
            }
        }
        function gridReload() {
            if (timeoutHnd) {
                clearTimeout(timeoutHnd);
				timeoutHnd = null;
            }

			if ($('#grid').jqGrid('getGridParam' ,'postData' ) != undefined) {
				$('#grid').jqGrid('setGridParam',{postData:{'searchField':'file_no'} });
				$('#grid').jqGrid('setGridParam',{postData:{'searchString':$('#grid_search_field').val()} });
				$('#grid').jqGrid('setGridParam',{postData:{'searchOper':'bw'} });
				
				//$($("#grid").navGrid("#pager")[0]).prop('p').postData.searchField = "file_no";
				//$($("#grid").navGrid("#pager")[0]).prop('p').postData.searchString = $('#grid_search_field').val();
				//$($("#grid").navGrid("#pager")[0]).prop('p').postData.searchOper = "bw";
				jQuery("#grid").trigger("reloadGrid");

				delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchField'];
				delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchString'];
				delete jQuery('#grid').jqGrid('getGridParam' ,'postData' )['searchOper'];
			}

            //var searchField = "file_no";
            //var searchString = $('#grid_search_field').val();
			//var searchOper = "bw";
			
			//myCustomSearch.triggerSearch();
			
			//jQuery("#grid").jqGrid('searchGrid', options );

			
            //jQuery("#grid").setGridParam({ url: "json_db_crud_pdo.php?searchField=" + searchField + "&searchString=" + searchString + "&searchOper=" + searchOper, page: 1 }).trigger("reloadGrid");
			//jQuery("#grid").setGridParam({ url: "json_db_crud_pdo.php", page: 1 });

			//$("#grid").jqGrid("setColProp", "file_no", { searchoptions: { sopt: ['cn']} }).trigger("reloadGrid");
            //jQuery("#grid").setGridParam({ url: "Checkup/List?search=" + search, page: 1 }).trigger("reloadGrid");
        }

        function enableAutosubmit(state) {
            flAuto = state;
            jQuery("#gridSubmitButton").attr("disabled", state);
			$('#griid_search_field').focus();
        } 