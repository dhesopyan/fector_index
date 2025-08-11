$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm filter-input"><option value="UNFINISHED">UNFINISHED</option><option value="FINISHED">FINISHED</option></select>')
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
            { "sName": "DealNumber", "sWidth": "20%" },
            { "sName": "Reason", "sWidth": "15%" },
            { "sName": "AccNum", "sWidth": "15%" },
            { "sName": "AccName", "sWidth": "20%" },
			{ "sName": "BranchId", "sWidth": "20%" },
			{ "sName": "TransactionType", "sWidth": "15%" },
			{ "sName": "CurrencyDeal", "sWidth": "20%" },
            { "sName": "AmountCustomer", "sWidth": "20%" },
			{ "sName": "DealDate", "sWidth": "15%" },
            { "sName": "AmountDeal", "sWidth": "15%" },
            { "sName": "AmountTransaction", "sWidth": "15%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sWidth": "20%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            $('td:eq(1)', nRow).addClass("hidden");

            var id = $('td:eq(0)', nRow).text();
            var controller = '<div><a href="#" data-toggle="modal" data-target="#Modal" id="CloseDeal" title="Close Deal"><span class="btn btn-info btn-sm fa fa-pencil"></span><a>' +
                '<input id="id" name="id" type="hidden" value="' + id + '">' +
                 '</form>';

            $('td:eq(12)', nRow).html(controller);

            if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
                $('.dataTables_paginate').css("display", "block");
            } else {
                $('.dataTables_paginate').css("display", "none");
            }
        }
    });
    $("#GvHeader").after($("#GvFilter"));
    $("#BtnFilter").bind("click", function () {
        $("#filterDDL option[value='UNFINISHED']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#GvFilter th input:eq(2)').val());
        table.column(3).search($('#GvFilter th input:eq(3)').val());
        table.column(4).search($('#GvFilter th input:eq(4)').val());
        table.column(5).search($('#GvFilter th input:eq(5)').val());
        table.column(6).search($('#GvFilter th input:eq(6)').val());
        table.column(7).search($('#GvFilter th input:eq(7)').val());
        table.column(8).search($('#GvFilter th input:eq(8)').val());
        table.column(9).search($('#GvFilter th input:eq(9)').val());
        table.column(10).search($('#GvFilter th input:eq(10)').val());
        table.column(11).search($("#filterDDL option:selected").val()).draw();
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $('#GvFilter th input:eq(2)').val("");
        $('#GvFilter th input:eq(3)').val("");
        $('#GvFilter th input:eq(4)').val("");
        $('#GvFilter th input:eq(5)').val("");
        $('#GvFilter th input:eq(6)').val("");
        $('#GvFilter th input:eq(7)').val("");
        $('#GvFilter th input:eq(8)').val("");
        $('#GvFilter th input:eq(9)').val("");
        $('#GvFilter th input:eq(10)').val("");
        $("#filterDDL option[value='UNFINISHED']").attr("selected", "selected");
        $('#filterDDL option:selected').val("UNFINISHED");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
    $('#myTable tbody').on('click', '#CloseDeal', function () {
        document.getElementById('DealNumber').value = $(this).parent().parent().parent().children().html();
    });
    $("#btnSave").click(function () {
        CloseDeal($("#DealNumber").val(), $("#CloseDealReason").val());
    });
    $("#btnCancel").click(function () {
        $("#DealNumber").val("");
        $("#CloseDealReason").val("");
    });
    function CloseDeal(DealNumber, Reason) {
        $.ajax({
            type: 'POST',
            url: 'CloseDeal',
            data: { DealNumber: DealNumber, Reason: Reason },
            cache: false,
            success: function (data) {
            }
        });
    };
});