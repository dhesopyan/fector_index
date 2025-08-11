$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
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
			'bSortable': false, 'aTargets': [6] //Disable sort for last column
		}],
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "DocumentId", "sWidth": "20%" },
            { "sName": "Description", "sWidth": "45%" },
            { "sName": "DocumentType", "sWidth": "20%" },
            { "sName": "CustomerType", "sWidth": "20%" },
            { "sName": "PurposeId", "sWidth": "30%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sName": "Task", "sWidth": "15%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
			$('td', nRow).addClass("GvRow");
			$('td:eq(6)', nRow).addClass("hidden-xs");
			var id = $('td:eq(0)', nRow).text();
			var status = $('td:eq(5)', nRow).text();
			var controller = '<div><form id="formApprove" action="' + baseUrl + 'MappingDocument/Approve" method="post"  class="btn btn-info btn-sm fa fa-check" title="Approve">' +
                             '<input id="id" name="id" type="hidden" value="' + id + '">' +
                             '</form>';
			controller += '<form id="formReject" action="' + baseUrl + 'MappingDocument/Reject" method="post"  class="btn btn-info btn-sm fa fa-remove" title="Reject">' +
                          '<input id="id" name="id" type="hidden" value="' + id + '">' +
                          '</form><div>';

			$('td:eq(6)', nRow).html(controller);

			if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
				$('.dataTables_paginate').css("display", "block");
			} else {
				$('.dataTables_paginate').css("display", "none");
			}
		}
	});
	$("#GvHeader").after($("#GvFilter"));
	$("#BtnFilter").bind("click", function () {
		table.column(0).search($('#GvFilter th input:eq(0)').val());
		table.column(1).search($('#GvFilter th input:eq(1)').val());
		table.column(2).search($('#GvFilter th input:eq(2)').val());
		table.column(3).search($('#GvFilter th input:eq(3)').val());
		table.column(4).search($('#GvFilter th input:eq(4)').val());
		table.column(5).search($('#GvFilter th input:eq(5)').val()).draw();
	});
	$("#BtnClear").bind("click", function () {
		$('#GvFilter th input:eq(0)').val("");
		$('#GvFilter th input:eq(1)').val("");
		$('#GvFilter th input:eq(2)').val("");
		$('#GvFilter th input:eq(3)').val("");
		$('#GvFilter th input:eq(4)').val("");
		$('#GvFilter th input:eq(5)').val("");
		$("#BtnFilter").trigger("click");
	});
	$(".filter-input").on("keypress", function (event) {
		if (event.which == 13) {
			$("#BtnFilter").trigger("click");
		}
	});
	//$('#myTable tbody').on("click", "#formDelete", function () {
	//    var obj = $(this);
	//    bootbox.confirm("Are you sure want to delete this data?", function (result) {
	//        if (result) {
	//            obj.submit();
	//        }
	//    });
	//});
	$('#myTable tbody').on("click", "#formApprove", function () {
	    var obj = $(this);
	    bootbox.confirm("Are you sure want to approve this mapping document?", function (result) {
	        if (result) {
	            obj.submit();
	        }
	    });
	});
	$('#myTable tbody').on("click", "#formReject", function () {
	    var obj = $(this);
	    bootbox.confirm("Are you sure want to reject this mapping document?", function (result) {
	        if (result) {
	            obj.submit();
	        }
	    });
	});
});