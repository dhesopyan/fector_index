$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
	var table = $('#myTable').DataTable({
		"autoWidth": false,
		"info": false,
		"sAjaxSource": ajaxHandler,
		"bLengthChange": false,
		"oLanguage": {
			"sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
		},
		"aoColumnDefs": [{
			'bSortable': false, 'aTargets': [9] //Disable sort for last column
		}],
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "RateId", "sWidth": "0%" },
            { "sName": "TDate", "sWidth": "15%" },
            { "sName": "TTime", "sWidth": "15%" },
            { "sName": "TTSellRate", "sWidth": "15%" },
            { "sName": "TTBuyRate", "sWidth": "15%" },
            { "sName": "BNSellRate", "sWidth": "15%" },
            { "sName": "BNBuyRate", "sWidth": "15%" },
            { "sName": "ClosingRate", "sWidth": "15%" },
            { "sName": "Status", "sWidth": "10%" },
            { "sWidth": "15%" },
            { "sWidth": "15%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('td', nRow).addClass("GvRow");
		    $('td:eq(0)', nRow).addClass("hidden");
		    var rateid = $('td:eq(0)', nRow).text();
		    var controller = '<div><form id="formApprove" action="' + baseUrl + 'ExchangeRate/Approve" method="post"  class="btn btn-info btn-sm fa fa-check" title="Approve">' +
                             '<input id="id" name="id" type="hidden" value="' + rateid + '">' +
                             '</form>';
			controller += '<form id="formReject" action="' + baseUrl + 'ExchangeRate/Reject" method="post"  class="btn btn-info btn-sm fa fa-remove" title="Reject">' +
                          '<input id="id" name="id" type="hidden" value="' + rateid + '">' +
                          '</form><div>';
			$('td:eq(10)', nRow).html(controller);

			if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
				$('.dataTables_paginate').css("display", "block");
			} else {
				$('.dataTables_paginate').css("display", "none");
			}
		}
	});
	$("#GvHeader").after($("#GvFilter"));
	$("#GvHeader2").after($("#GvFilter"));
	$("#BtnFilter").bind("click", function () {
		table.column(0).search($('#GvFilter th input:eq(0)').val());
		table.column(1).search($('#GvFilter th input:eq(1)').val());
		table.column(2).search($('#GvFilter th input:eq(2)').val()).draw();
	});
	$("#BtnClear").bind("click", function () {
		$('#GvFilter th input:eq(0)').val("");
		$('#GvFilter th input:eq(1)').val("");
		$('#GvFilter th input:eq(2)').val("");
		$("#BtnFilter").trigger("click");
	});
	$(".filter-input").on("keypress", function (event) {
		if (event.which == 13) {
			$("#BtnFilter").trigger("click");
		}
	});
	$('#myTable tbody').on("click", "#formApprove", function () {
	    var obj = $(this);
	    bootbox.confirm("Are you sure want to approve this exchange rate?", function (result) {
	        if (result) {
	            obj.submit();
	        }
	    });
	});
	$('#myTable tbody').on("click", "#formReject", function () {
	    var obj = $(this);
	    bootbox.confirm("Are you sure want to reject this exchange rate?", function (result) {
	        if (result) {
	            obj.submit();
	        }
	    });
	});
});