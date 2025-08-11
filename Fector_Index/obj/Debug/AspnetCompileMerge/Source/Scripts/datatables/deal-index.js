$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
	$('#filterDDL').each(function () {
	    $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm">' +
                            '<option value="">ALL</option>' +
                            '<option value="ACTIVE">ACTIVE</option>' +
                            '<option value="INACTIVE">INACTIVE</option>' +
                            '<option value="REJECTED">REJECTED</option>' +
                    '</select>')
	});
	var table = $('#myTable').DataTable({
		"sDom": '<"top"i>rt<"bottom"l><"clear">',
		"autoWidth": false,
		"info": false,
		"bSort": false,
		"sAjaxSource": ajaxHandler,
		"bLengthChange": false,
		"oLanguage": {
			"sProcessing": "<div class='text-center'><img src='" + baseUrl + "/Content/Images/loading.gif'>"
		},
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "ID", "sWidth": "11%" },
            { "sName": "DealNumber", "sWidth": "20%" },
            { "sName": "AccNum", "sWidth": "16%" },
            { "sName": "AccName", "sWidth": "18%" },
            { "sName": "TransactionType", "sWidth": "17%" },
            { "sName": "CurrencyDeal", "sWidth": "19%" },
            { "sName": "AmountCustomer", "sWidth": "20%" },
			{ "sName": "DealDate", "sWidth": "16%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sName": "Task", "sWidth": "15%" },
            { "sName": "BranchId", "sWidth": "15%" },
            { "sName": "TransactionType", "sWidth": "15%" },
            { "sName": "DealType", "sWidth": "15%" },
            { "sName": "CurrencyDeal", "sWidth": "15%" },
            { "sName": "DealRate", "sWidth": "15%" },
            { "sName": "AmountDeal", "sWidth": "15%" },
            { "sName": "CurrencyCustomer", "sWidth": "15%" },
            { "sName": "AmountCustomer", "sWidth": "15%" },
            { "sName": "RateCustomer", "sWidth": "15%" },
            { "sName": "DealPeriod", "sWidth": "15%" },
            { "sName": "DealDate", "sWidth": "15%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sName": "Task", "sWidth": "15%" },

		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('td', nRow).addClass("GvRow");
		    $('td:eq(0)', nRow).addClass("hidden");
		    $('td:eq(6)', nRow).addClass("hidden");
		    $('td:eq(8)', nRow).addClass("hidden");
		    for (i = 10; i <= 22; i++) {
		        $('td:eq(' + i + ')', nRow).addClass("hidden");
		    }

			var accno = $('td:eq(2)', nRow).text();
			var name = $('td:eq(3)', nRow).text().replace("'", "%22");
			var branchid = $('td:eq(10)', nRow).text();
			var transtype = $('td:eq(11)', nRow).text();
			var dealtype = $('td:eq(12)', nRow).text();
			var currencydeal = $('td:eq(13)', nRow).text();
			var dealrate = $('td:eq(14)', nRow).text();
			var amountdeal = $('td:eq(15)', nRow).text();
			var currencycustomer = $('td:eq(16)', nRow).text();
			var amountcustomer = $('td:eq(17)', nRow).text();
			var ratecustomer = $('td:eq(18)', nRow).text();
			var dealperiod = $('td:eq(19)', nRow).text();
			var dealdate = $('td:eq(20)', nRow).text();
			
		    //var controller = '<div><a href="#" title="Select" onclick="returnValue("' + id + '","' + name + '")"><span class="btn btn-info btn-sm fa fa-check"></span><a>';
			var controller = '<div><a href="#" title="Select" onclick="returnValue(\'' + accno + '\',\'' + name + '\',\'' + branchid + '\',\'' + transtype + '\',\'' + dealtype + '\',\'' + currencydeal + '\',\'' + amountdeal + '\',\'' + dealrate + '\',\'' + currencycustomer + '\',\'' + amountcustomer + '\',\'' + ratecustomer + '\',\'' + dealperiod + '\',\'' + dealdate + '\')"><span class="btn btn-info btn-sm fa fa-check"></span><a>';
			$('td:eq(9)', nRow).html(controller);

			if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
				$('.dataTables_paginate').css("display", "block").addClass("text-center");
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
	    $("#filterDDL option[value='']").attr("selected", "selected");
	    $('#filterDDL option:selected').val("");
	    $("#BtnFilter").trigger("click");

		$("#BtnFilter").trigger("click");
	});
	$(".filter-input").on("keypress", function (event) {
		if (event.which == 13) {
			$("#BtnFilter").trigger("click");
		}
	});
});