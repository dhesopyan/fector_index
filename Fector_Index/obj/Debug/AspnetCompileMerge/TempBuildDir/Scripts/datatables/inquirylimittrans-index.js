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
		"bProcessing": true,
		"bServerSide": true,
		"aoColumns": [
            { "sName": "CIF", "sWidth": "40%" },
            { "sName": "AccName", "sWidth": "40%" },
            { "sWidth": "20%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
		    $('td', nRow).addClass("GvRow");

		    var id = $('td:eq(0)', nRow).text();
		    var name = $('td:eq(1)', nRow).text();

		    var controller = '<div><a href="' + baseUrl + 'InquiryLimitTrans/BrowseDetailDeal?cif=' + encodeURIComponent(id) + '&accname=' + encodeURIComponent(name) + '" id="view" title="Browse"><span class="btn btn-info btn-sm fa fa-eye"></span><a>';
            
		    //controller += '<a href="' + baseUrl + 'InquiryLimitTrans/RptDetailInquiryLimit?cif=' + encodeURIComponent(id) + '&accname=' + encodeURIComponent(name) + '" title="print"><span class="btn btn-info btn-sm fa fa-print"></span><a>';

		    $('td:eq(2)', nRow).html(controller);

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
$(function () {
    $('#myTable tbody').on('click', '#view', function (e) {
        e.preventDefault();

        var link = $(this).attr('href');

        $(this).attr('href', link);
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });

    $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });
});