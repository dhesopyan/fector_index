$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="">ALL</option>' +
                            '<option value="1">PROCESS</option>' +
                            '<option value="0">ACTIVE</option>' +
                            '<option value="2">INACTIVE</option>' +
                            '<option value="-1">DELETED</option>' +
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
            'bSortable': false, 'aTargets': [4] //Disable sort for last column
        }],
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "RefNo", "sWidth": "15%" },
            { "sName": "TDate", "sWidth": "20%" },
            { "sName": "AccNo", "sWidth": "20%" },
            { "sName": "AccName", "sWidth": "20%" },
            { "sName": "CoreCurrId", "sWidth": "20%" },
            { "sName": "BranchId", "sWidth": "20%" },
            { "sName": "Amount", "sWidth": "20%" },
            { "sName": "Time", "sWidth": "20%" },
            { "sName": "Rate", "sWidth": "20%" },
            { "sName": "FlagLHBU", "sWidth": "20%" },
            { "sWidth": "20%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            var id = $('td:eq(0)', nRow).text();
            $('td:eq(0)', nRow).addClass("hidden");
            var controller = '<div><input type="checkbox" name="chkSelect" value=\'' + id + '\')></input>';
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
        $("#filterDDL option[value='']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#GvFilter th input:eq(2)').val());
        table.column(3).search($('#GvFilter th input:eq(3)').val());
        table.column(4).search($('#GvFilter th input:eq(4)').val());
        table.column(5).search($('#GvFilter th input:eq(5)').val());
        table.column(6).search($('#GvFilter th input:eq(6)').val());
        table.column(7).search($('#GvFilter th input:eq(7)').val());
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
        $('#GvFilter th input:eq(7)').val("");
        $("#filterDDL option[value='']").attr("selected", "selected");
        $('#filterDDL option:selected').val("");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
});