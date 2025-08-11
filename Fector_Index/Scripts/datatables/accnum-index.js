$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
    var table = $('#myTable').DataTable({
        "sDom": '<"top"i>rt<"bottom"lp><"clear">',
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
            { "sName": "AccNo", "sWidth": "30%" },
            { "sName": "Name", "sWidth": "50%" },
            { "sWidth": "20%" },
            { "sName": "TOMLimit", "sWidth": "50%" },
            { "sName": "SPOTLimit", "sWidth": "50%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('td', nRow).addClass("GvRow");
		    $('td:eq(3)', nRow).addClass("hidden");
		    $('td:eq(4)', nRow).addClass("hidden");

			var id = $('td:eq(0)', nRow).text();
			var name = $('td:eq(1)', nRow).text().replace("'","%22");
			var tomlimit = $('td:eq(3)', nRow).text();
			var spotlimit = $('td:eq(4)', nRow).text();

			var controller = '<div><a href="#" title="Select" onclick="returnValue(\'' + id + '\',\'' + name + '\',\'' + tomlimit + '\',\'' + spotlimit + '\')"><span class="btn btn-info btn-sm fa fa-check"></span><a>';
			$('td:eq(2)', nRow).html(controller);

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