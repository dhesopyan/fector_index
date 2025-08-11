$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%">' +
                        '<option value="">ALL</option>' +
                        '<option value="ACTIVE">ACTIVE</option>' + 
                        '<option value="REJECTED">REJECTED</option>' + 
                        '<option value="EDIT - PENDING">EDIT - PENDING</option>' + 
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
            { "sName": "AccNo", "sWidth": "15%" },
            { "sName": "CIF", "sWidth": "15%" },
            { "sName": "Name", "sWidth": "35%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sWidth": "10%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            var id = $('td:eq(0)', nRow).text();
            var cif = $('td:eq(1)', nRow).text();
            var status = $('td:eq(3)', nRow).text();
            var controller = '<div><a href="' + baseUrl + 'Account/Edit?id=' + id + '&cif=' + cif + '" title="Edit"><span class="btn btn-info btn-sm fa fa-edit"></span><a>';
            
            if (status.indexOf("PENDING") == -1)
            {
                $('td:eq(4)', nRow).html(controller);
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
        table.column(3).search($('#filterDDL option:selected').val()).draw();
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $('#GvFilter th input:eq(2)').val("");
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