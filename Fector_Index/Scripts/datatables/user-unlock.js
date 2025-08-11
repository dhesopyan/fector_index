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
        "aoColumnDefs": [{
            'bSortable': false, 'aTargets': [6] 
        }],
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "LogId", "sWidth": "0%" },
            { "sName": "Username", "sWidth": "10%" },
            { "sName": "FullName", "sWidth": "30%" },
            { "sName": "Role", "sWidth": "10%" },
            { "sName": "Branch", "sWidth": "20%" },
            { "sName": "LoginTime", "sWidth": "15%" },
            { "sWidth": "15%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            $('td:eq(0)', nRow).addClass("hidden");
            var id = $('td:eq(0)', nRow).text();
            var controller = '<div>' +
                             '<form id="formUnlock" action="' + baseUrl + 'User/UnlockUser" method="post"  class="btn btn-info btn-sm fa fa-unlock" title="Unlock">' +
                             '<input id="id" name="id" type="hidden" value="' + id + '">' +
                             '</form>' + 
                             '<div>';
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
    $('#myTable tbody').on("click", "#formUnlock", function () {
        var obj = $(this);
        bootbox.confirm("Are you sure want to force this user to logout?", function (result) {
            if (result) {
                obj.submit();
            }
        });
    });
});