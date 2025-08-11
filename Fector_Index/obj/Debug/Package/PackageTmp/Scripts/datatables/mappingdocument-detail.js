$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
	var table = $('#myTable').DataTable({
	    "iDisplayLength": 999,
		"autoWidth": false,
		"info": false,
		"sAjaxSource": ajaxHandler,
		"bLengthChange": false,
		"oLanguage": {
			"sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
		},
		"aoColumnDefs": [{
			'bSortable': false, 'aTargets': [2] //Disable sort for last column
		}],
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "DocumentId", "sWidth": "20%" },
            { "sName": "Description", "sWidth": "45%" },
            { "sName": "Checklist", "sWidth": "15%" },
            { "sName": "Task", "sWidth": "15%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('.dataTables_paginate').css("display", "none");
		}
	});
	$("#GvHeader").after($("#GvFilter"));
	$("#BtnFilter").bind("click", function () {
		table.column(0).search($('#GvFilter th input:eq(0)').val());
		table.column(1).search($('#GvFilter th input:eq(1)').val()).draw();
	});
	$("#BtnClear").bind("click", function () {
		$('#GvFilter th input:eq(0)').val("");
		$('#GvFilter th input:eq(1)').val("");
		$("#BtnFilter").trigger("click");
	});
	$(".filter-input").on("keypress", function (event) {
		if (event.which == 13) {
			$("#BtnFilter").trigger("click");
		}
	});
});