$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
	$('#filterDDL').each(function () {
	    $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm">' +
                            '<option value="ALL">ALL</option>' +
                            '<option value="CREATE - PENDING">CREATE - PENDING</option>' +
                            '<option value="EDIT - PENDING">EDIT - PENDING</option>' +
                            '<option value="DELETE - PENDING">DELETE - PENDING</option>' +
                    '</select>')
	});
	var table = $('#myTable').DataTable({
		"autoWidth": false,
		"info": false,
		"sAjaxSource": ajaxHandler,
		"bLengthChange": false,
		"oLanguage": {
			"sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
		},
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "ID", "sWidth": "7%" },
            { "sName": "DealNumber", "sWidth": "11%" },
            { "sName": "AccNum", "sWidth": "11%" },
            { "sName": "AccName", "sWidth": "15%" },
            { "sName": "branch", "sWidth": "11%" },
            { "sName": "TransactionType", "sWidth": "10%" },
            { "sName": "CurrencyDeal", "sWidth": "15%" },
            { "sName": "AmountCustomer", "sWidth": "15%" },
			{ "sName": "DealDate", "sWidth": "11%" },
            { "sName": "Status", "sWidth": "11%" },
            { "sName": "Task", "sWidth": "11%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('td', nRow).addClass("GvRow");
		    var id = $('td:eq(0)', nRow).text();
		    $('td:eq(0)', nRow).addClass("hidden");
		    var status = $('td:eq(9)', nRow).text();
		    var controller = '<div><a href="' + baseUrl + 'TransactionDeal/Process?id=' + id + '" title="Process"><span class="btn btn-info btn-sm fa fa-eye"></span><a>';

			$('td:eq(10)', nRow).html(controller);

			if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
				$('.dataTables_paginate').css("display", "block");
			} else {
				$('.dataTables_paginate').css("display", "none");
			}
		}
	});
	$("#GvHeader").after($("#GvFilter"));
	$("#BtnFilter").bind("click", function () {
	    $("#filterDDL option[value='ALL']").removeAttr("selected");
	    table.column(0).search($('#GvFilter th input:eq(0)').val());
	    table.column(1).search($('#GvFilter th input:eq(1)').val());
	    table.column(2).search($('#GvFilter th input:eq(2)').val());
	    table.column(3).search($('#GvFilter th input:eq(3)').val());
	    table.column(4).search($('#GvFilter th input:eq(4)').val());
	    table.column(5).search($('#GvFilter th input:eq(5)').val());
	    table.column(6).search($('#GvFilter th input:eq(6)').val());
	    table.column(7).search($('#filterDDL option:selected').val()).draw();
	});
	$("#BtnClear").bind("click", function () {
	    $('#GvFilter th input:eq(0)').val("");
	    $('#GvFilter th input:eq(1)').val("");
	    $('#GvFilter th input:eq(2)').val("");
	    $('#GvFilter th input:eq(3)').val("");
	    $('#GvFilter th input:eq(4)').val("");
	    $('#GvFilter th input:eq(5)').val("");
	    $('#GvFilter th input:eq(6)').val("");
	    $("#filterDDL option[value='ALL']").attr("selected", "selected");
	    $('#filterDDL option:selected').val("");
		$("#BtnFilter").trigger("click");
	});
	$(".filter-input").on("keypress", function (event) {
		if (event.which == 13) {
			$("#BtnFilter").trigger("click");
		}
	});
});