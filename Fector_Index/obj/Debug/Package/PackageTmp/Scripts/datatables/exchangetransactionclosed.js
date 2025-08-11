$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="ALL">ALL</option>' +
                            '<option value="CLOSED">CLOSED</option>' +
                            '<option value="ACTIVE">ACTIVE</option>' +
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
            $('td:eq(8)', nRow).addClass("hidden");
            $('td:eq(11)', nRow).addClass("hidden");
            var id = $('td:eq(0)', nRow).text();
            var dealnumber = $('td:eq(1)', nRow).text();
            var status = $('td:eq(9)', nRow).text();
            var branchid = $('td:eq(11)', nRow).text();
            var controller = ''

            if (status == 'CLOSED') {
                controller += '<div><a href="#" data-toggle="modal" data-target="#Modal" id="ViewClosed" title="View"><span class="btn btn-info btn-sm fa fa-eye"></span><a>' +
                                '<input id="id" name="id" type="hidden" value="' + id + '">' +
                                '</form>';
            }
            else {
                controller += '<div><a href="#" data-toggle="modal" data-target="#Modal" id="CloseTransaction" title="Close Transaction"><span class="btn btn-info btn-sm fa fa-pencil"></span><a>' +
                            '<input id="id" name="id" type="hidden" value="' + id + '">' +
                             '</form>';
            }

            if (status.indexOf("PENDING") == -1) {
                $('td:eq(10)', nRow).html(controller);
            }

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
        table.column(0).search($('#GvFilter th input:eq(1)').val());
        table.column(1).search($('#GvFilter th input:eq(2)').val());
        table.column(2).search($('#GvFilter th input:eq(3)').val());
        table.column(3).search($('#GvFilter th input:eq(4)').val());
        table.column(4).search($('#GvFilter th input:eq(5)').val());
        table.column(5).search($('#GvFilter th input:eq(6)').val());
        table.column(6).search($('#filterDDL option:selected').val()).draw();
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
        $("#filterDDL option[value='ALL']").attr("selected", "selected");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
    $('#myTable tbody').on('click', '#CloseTransaction', function () {
        $("#DealNumber").removeAttr("readonly");
        $("#CloseTransactionReason").removeAttr("readonly");
        $("#btnSave").removeClass('hidden');
        document.getElementById('TransNum').value = $(this).parent().parent().parent().children().html();
        document.getElementById('DealNumber').value = $(this).parent().parent().parent().children().next().html();
        document.getElementById('CloseTransactionReason').value = $(this).parent().parent().parent().children().next().next().next().next().next().next().next().next().next().next().next().html();
    });

    $('#myTable tbody').on('click', '#ViewClosed', function () {
        $("#DealNumber").attr("readonly", "readonly");
        $("#CloseTransactionReason").attr("readonly", "readonly");
        $("#btnSave").addClass('hidden');
        document.getElementById('TransNum').value = $(this).parent().parent().parent().children().html();
        document.getElementById('DealNumber').value = $(this).parent().parent().parent().children().next().html();
        document.getElementById('CloseTransactionReason').value = $(this).parent().parent().parent().children().next().next().next().next().next().next().next().next().next().next().next().html();
    });
    //$("#btnSave").click(function () {
    //    CloseDeal($("#TransNum").val(), $("#CloseTransactionReason").val());
    //});
    $("#btnCancel").click(function () {
        $("#TransNum").val("");
        $("#DealNumber").val("");
        $("#CloseTransactionReason").val("");
    });
    //function CloseDeal(TransNum, Reason) {
    //    $.ajax({
    //        type: 'POST',
    //        url: 'CloseTransaction',
    //        data: { TransNum: TransNum, Reason: Reason },
    //        cache: false,
    //        success: function (data) {
    //        }
    //    });
    //};
});