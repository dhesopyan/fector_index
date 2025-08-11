$(document).ready(function () {
    $('#myTable thead tr#GvFilter th.filter').each(function () {
        var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
        $(this).html('<input type="text" id="' + $(this).attr("data-id") + '" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
    });
    $('#filterDDL').each(function () {
        $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm filter-input"><option value="NOT UPLOADED">NOT UPLOADED</option><option value="UPLOADED">UPLOADED</option></select>')
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
            { "sName": "AccNum", "sWidth": "30%" },
            { "sName": "CIF", "sWidth": "30%" },
            { "sName": "AccName", "sWidth": "30%" },
            { "sName": "Status", "sWidth": "20%" },
            { "sName": "FileDirectory", "sWidth": "30%" },
            { "sWidth": "20%" },
        ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
            $('td', nRow).addClass("GvRow");
            $('td:eq(4)', nRow).addClass("hidden");
            var id = $('td:eq(0)', nRow).text();
            var status = $('td:eq(3)', nRow).text();
            //var controller = '<form id="formUpload" action="' + baseUrl + 'UploadNPWP/SendAccNo" method="post"  class="btn btn-info btn-sm fa fa-upload" title="Upload NPWP">' +
            //                  '<input id="id" name="id" type="hidden" value="' + id + '">' +
            //                  '</form>';
            var controller = '<div><a href="#" id="upload" title="Upload"><span class="btn btn-info btn-sm fa fa-upload"></span><a>' +
                '<input id="accno" name="accno" type="hidden" value="' + id + '">' +
                 '</form>';
           
            if (status == 'UPLOADED') {
                controller += '<form id="view" class="btn btn-info btn-sm fa fa-eye" title="View">' +
                             '<input id="accno" name="accno" type="hidden" value="' + id + '">' +
                             '</form>';
            }
            else{
                controller += '';
            }

            $('td:eq(5)', nRow).html(controller);

            if (Math.ceil((this.fnSettings().fnRecordsDisplay()) / this.fnSettings()._iDisplayLength) > 1) {
                $('.dataTables_paginate').css("display", "block");
            } else {
                $('.dataTables_paginate').css("display", "none");
            }
        }
    });
    $("#GvHeader").after($("#GvFilter"));
    $("#BtnFilter").bind("click", function () {
        $("#filterDDL option[value='NotUploaded']").removeAttr("selected");
        table.column(0).search($('#GvFilter th input:eq(0)').val());
        table.column(1).search($('#GvFilter th input:eq(1)').val());
        table.column(2).search($('#GvFilter th input:eq(2)').val());
        table.column(3).search($('#filterDDL option:selected').val()).draw();
        table.column(4).search($('#GvFilter th input:eq(4)').val()).draw();
        
    });
    $("#BtnClear").bind("click", function () {
        $('#GvFilter th input:eq(0)').val("");
        $('#GvFilter th input:eq(1)').val("");
        $('#GvFilter th input:eq(2)').val("");
        $('#GvFilter th input:eq(3)').val("");
        $("#filterDDL option[value='NotUploaded']").attr("selected", "selected");
        $('#filterDDL option:selected').val("NotUploaded");
        $("#BtnFilter").trigger("click");
    });
    $(".filter-input").on("keypress", function (event) {
        if (event.which == 13) {
            $("#BtnFilter").trigger("click");
        }
    });
    $('#myTable tbody').on('click', '#upload', function () {
        var accno = $(this).parent().parent().parent().children().html();
        var link = $(this).parent().parent().parent().children().next().next().next().next().html();

        document.getElementById("lastfiledirectory").value = link;
        document.getElementById("accno").value = accno;
        document.getElementById("File").click();
    });
    $('#myTable tbody').on('click', '#view', function () {
        var link = $(this).parent().parent().parent().parent().children().next().next().next().next().html();
        //alert(link);
        //link = link.substring(link.lastIndexOf("/") + 1);
        
        window.open(link, '_blank');
        //SendAccNo($(this).parent().parent().parent().children().next().next().next().html());
        //document.getElementById("File").click();
    });
    $('#File').change(function () {
        $('#frm').submit();
    });
});