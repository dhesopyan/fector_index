$(document).ready(function () {
	$('#myTable thead tr#GvFilter th.filter').each(function () {
		var title = $('#myTable thead tr.GvHeader th').eq($(this).index()).text().trim();
		$(this).html('<input type="text" class="form-control input-sm filter-input" style="width:100%" placeholder="Find ' + title + '" />');
	});
	$('#filterDDL').each(function () {
	    $(this).html('<select name="filterDDL" id="filterDDL" class="form-control input-sm" style="width:100%"> ' +
                            '<option value="">ALL</option>' +
                            '<option value="ACTIVE">ACTIVE</option>' +
                            '<option value="INACTIVE">INACTIVE</option>' +
                            '<option value="REJECTED">REJECTED</option>' +
                            '<option value="CREATE - PENDING">CREATE - PENDING</option>' +
                            '<option value="EDIT - PENDING">EDIT - PENDING</option>' +
                            '<option value="DELETE - PENDING">DELETE - PENDING</option>' +
                            '<option value="ACTIVE - PENDING">ACTIVE - PENDING</option>' +
                            '<option value="INACTIVE - PENDING">INACTIVE - PENDING</option>' +
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
            { "sName": "BICode", "sWidth": "10%" },
            { "sName": "Status", "sWidth": "10%" },
            { "sName": "Name", "sWidth": "35%" },
            { "sName": "Status", "sWidth": "15%" },
            { "sWidth": "15%" },
		],
		"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
			$('td', nRow).addClass("GvRow");
			var id = $('td:eq(0)', nRow).text();
			var status = $('td:eq(3)', nRow).text();
			var controller = '<div><a href="' + baseUrl + 'BICode/Edit/' + id + '/" title="Edit"><span class="btn btn-info btn-sm fa fa-edit"></span><a>';

			if (status.indexOf("INACTIVE") > -1) {
				controller += '<form id="formActive" action="' + baseUrl + 'BICode/Active" method="post"  class="btn btn-info btn-sm fa fa-check" title="Active Data">' +
                              '<input id="id" name="id" type="hidden" value="' + id + '">' +
                              '</form>';
			}
			else {
				controller += '<form id="formInactive" action="' + baseUrl + 'BICode/Inactive" method="post"  class="btn btn-info btn-sm fa fa-close " title="Inactive Data">' +
                              '<input id="id" name="id" type="hidden" value="' + id + '">' +
                              '</form>';
			}
			controller += '<form id="formDelete" action="' + baseUrl + 'BICode/Delete" method="post"  class="btn btn-info btn-sm fa fa-trash-o" title="Delete">' +
                          '<input id="id" name="id" type="hidden" value="' + id + '">' +
                          '</form><div>';

			if (status.indexOf("PENDING") == -1) {
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
	$('#myTable tbody').on("click", "#formDelete", function () {
		var obj = $(this);
		bootbox.confirm("Are you sure want to delete this data?", function (result) {
			if (result) {
				obj.submit();
			}
		});
	});
	$('#myTable tbody').on("click", "#formInactive", function () {
		var obj = $(this);
		bootbox.confirm("Are you sure want to inactive this data?", function (result) {
			if (result) {
				obj.submit();
			}
		});
	});
	$('#myTable tbody').on("click", "#formActive", function () {
		var obj = $(this);
		bootbox.confirm("Are you sure want to active this data?", function (result) {
			if (result) {
				obj.submit();
			}
		});
	});
});