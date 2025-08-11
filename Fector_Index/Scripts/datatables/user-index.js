$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="ACTIVE">ACTIVE</option>' +
                            '<option value="ALL">ALL</option>' +
                            '<option value="INACTIVE">INACTIVE</option>' +
                            '<option value="CREATE - PENDING">CREATE - PENDING</option>' +
                            '<option value="EDIT - PENDING">EDIT - PENDING</option>' +
                            '<option value="DELETE - PENDING">DELETE - PENDING</option>' +
                            '<option value="RESET PWD - PENDING">RESET PWD - PENDING</option>' +
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
            'bSortable': false, 'aTargets': [6] //Disable sort for last column
        }],
        "bProcessing": true,
        "bServerSide": true,
        "aoColumns": [
            { "sName": "Username", "sWidth": "10%" },
            { "sName": "FullName", "sWidth": "20%" },
            { "sName": "Role", "sWidth": "10%" },
            { "sName": "Branch", "sWidth": "17%" },
            { "sName": "Limit", "sWidth": "15%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sWidth": "20%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            $('td:eq(2)', nRow).addClass("hidden-xs");
            var id = $('td:eq(0)', nRow).text();
            var status = $('td:eq(5)', nRow).text();
            var controller = '<div><a href="' + baseUrl + 'User/Edit/' + id + '/" title="Edit"><span class="btn btn-info btn-sm fa fa-edit"></span><a>' +
                             '<form id="formResetPassword" action="' + baseUrl + 'User/ResetPassword" method="post"  class="btn btn-info btn-sm fa fa-refresh" title="Reset Password">' +
                             '<input id="id" name="id" type="hidden" value="' + id + '">' +
                             '</form>';
            if (status.indexOf("INACTIVE")>-1)
            {
                controller += '<form id="formActiveUser" action="' + baseUrl + 'User/Active" method="post"  class="btn btn-info btn-sm fa fa-check" title="Active User">' +
                              '<input id="id" name="id" type="hidden" value="' + id + '">' +
                              '</form>';
            }
            else
            {
                controller += '<form id="formInactiveUser" action="' + baseUrl + 'User/Inactive" method="post"  class="btn btn-info btn-sm fa fa-close " title="Inactive User">' +
                              '<input id="id" name="id" type="hidden" value="' + id + '">' +
                              '</form>';
            }
            controller += '<form id="formDelete" action="' + baseUrl + 'User/Delete" method="post"  class="btn btn-info btn-sm fa fa-trash-o" title="Delete">' +
                          '<input id="id" name="id" type="hidden" value="' + id + '">' +
                          '</form><div>';

            if (status.indexOf("PENDING") == -1) {
                $('td:eq(6)', nRow).html(controller);
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
        $("#filterDDL option[value='']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#GvFilter th input:eq(2)').val());
        table.column(3).search($('#GvFilter th input:eq(3)').val());
        table.column(4).search($('#GvFilter th input:eq(4)').val());
        table.column(5).search($('#GvFilter th option:selected').val()).draw();
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $('#GvFilter th input:eq(2)').val("");
        $('#GvFilter th input:eq(3)').val("");
        $('#GvFilter th input:eq(4)').val("");
        $("#filterDDL option[value='']").attr("selected", "selected");
        $('#filterDDL option:selected').val("");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
    $('#myTable tbody').on("click", "#formDelete", function () {
        var obj = $(this);
        bootbox.confirm("Are you sure want to delete this data?", function (result) {
            if (result) {
                obj.submit();
            }
        });
    });
    $('#myTable tbody').on("click", "#formInactiveUser", function () {
        var obj = $(this);
        bootbox.confirm("Are you sure want to inactive this user?", function (result) {
            if (result) {
                obj.submit();
            }
        });
    });
    $('#myTable tbody').on("click", "#formActiveUser", function () {
        var obj = $(this);
        bootbox.confirm("Are you sure want to active this user?", function (result) {
            if (result) {
                obj.submit();
            }
        });
    });
    $('#myTable tbody').on("click", "#formResetPassword", function () {
        var obj = $(this);
        bootbox.confirm("Are you sure want to reset this user's password?", function (result) {
            if (result) {
                obj.submit();
            }
        });
    });
});