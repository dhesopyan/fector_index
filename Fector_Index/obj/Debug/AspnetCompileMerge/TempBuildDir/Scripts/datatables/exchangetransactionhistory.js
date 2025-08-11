$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
	$('#filterDDL').each(function () {
	    $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="ACTIVE">ACTIVE</option>' +
                            '<option value="ALL">ALL</option>' +
                            '<option value="INACTIVE">INACTIVE</option>' +
                            '<option value="REJECTED">REJECTED</option>' +
                            '<option value="DEAL - PENDING">CREATE - PENDING</option>' +
                            '<option value="EDIT DEAL - PENDING">EDIT - PENDING</option>' +
                            '<option value="DELETE - PENDING">DELETE - PENDING</option>' +
                    '</select>')
	});
	$('#filterReconsile').each(function () {
	    $(this).html('<select name="filterReconsile" id="filterReconsile" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="0">No</option>' +
                            '<option value="1">Yes</option>' +
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
        "aoColumnDefs": [{
            'bSortable': false, 'aTargets': [8] //Disable sort for last column
        }],
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "TransNum", "sWidth": "21%" },
            { "sName": "DealNumber", "sWidth": "21%" },
            { "sName": "TDate", "sWidth": "21%" },
            { "sName": "AccNum", "sWidth": "21%" },
            { "sName": "AccName", "sWidth": "21%" },
            { "sName": "TransactionType", "sWidth": "21%" },
            { "sName": "RateType", "sWidth": "21%" },
			{ "sName": "SourceFunds", "sWidth": "21%" },
            { "sName": "FlagReconsile", "sWidth": "21%" },
            { "sName": "Status", "sWidth": "21%" },
            { "sName": "Task", "sWidth": "21%" },
            { "sName": "BranchId", "sWidth": "21%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            $('td:eq(0)', nRow).addClass("hidden");
            $('td:eq(6)', nRow).addClass("hidden");
            $('td:eq(11)', nRow).addClass("hidden");
            var id = $('td:eq(0)', nRow).text();
            var dealnumber = $('td:eq(1)', nRow).text();
            var status = $('td:eq(9)', nRow).text();
            var branchid = $('td:eq(11)', nRow).text();

            var controller = '<a href="' + baseUrl + 'ExchangeTransaction/ViewHistory?id=' + id + '" title="ViewHistory"><span class="btn btn-info btn-sm fa fa-eye"></span><a>';

            controller += '<form id="formView" action="' + baseUrl + 'ExchangeTransaction/PrintReport" method="post"  class="btn btn-info btn-sm fa fa-print" title="Print">' +
              '<input id="id" name="id" type="hidden" value="' + id + '">' +
              '<input id="dealnumber" name="dealnumber" type="hidden" value="' + dealnumber + '">' +
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
    $("#BtnFilter").bind("click", function () {
        $("#filterDDL option[value='ACTIVE']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#GvFilter th input:eq(2)').val());
        table.column(3).search($('#GvFilter th input:eq(3)').val());
        table.column(4).search($('#GvFilter th input:eq(4)').val());
        table.column(5).search($('#GvFilter th input:eq(5)').val());
        table.column(6).search($('#filterReconsile option:selected').val()).draw();
        table.column(7).search($('#filterDDL option:selected').val()).draw();
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $('#GvFilter th input:eq(2)').val("");
        $('#GvFilter th input:eq(3)').val("");
        $('#GvFilter th input:eq(4)').val("");
        $('#GvFilter th input:eq(5)').val("");
        ("#filterReconsile option[value='No']").attr("selected", "selected");
        $("#filterDDL option[value='ALL']").attr("selected", "selected");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
    $('#myTable tbody').on("click", "#formView", function () {
        var obj = $(this);
        obj.submit();
    });
});