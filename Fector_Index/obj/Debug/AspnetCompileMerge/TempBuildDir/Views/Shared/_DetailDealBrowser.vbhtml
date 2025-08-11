<html>
<head>
    <title>Detail Deal This Month</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <div class="col-md-6 ">
                    <label>CIF</label>
                    <input id="txtCIF" type="text" disabled="disabled" value="@ViewBag.cif" style="width:100%;" />
                </div>
                <div class="col-md-6 ">
                    <label>Acc. Name</label>
                    <input id="txtAccName" type="text" disabled="disabled" value="@ViewBag.accname" style="width:100%;" />
                </div>         
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                &nbsp;
            </div>
        </div>
        <div class="row">
            <div class="col-md-12">
                <table id="myTableDetail" class="table FectorTable">
                    <thead>
                        <tr id="GvFilterDetail" class="GvHeader">
                            <th class="hidden-xs filter" data-id="filterDate">
                                Value Date
                            </th>
                            <th class="hidden-xs filter">
                                Deal Number
                            </th>
                            <th class="hidden-xs filter">
                                Acc. Number
                            </th>
                            <th>
                                Original Currency
                            </th>
                            <th>
                                &nbsp;
                            </th>
                            <th>
                                <a id="BtnFilterDetail" class="btn btn-primary btn-sm">
                                    <span class="fa fa-search"></span>
                                </a>
                                <a id="BtnClearDetail" class="btn btn-primary btn-sm">
                                    <span class="fa fa-refresh"></span>
                                </a>
                            </th>
                        </tr>
                        <tr id="GvHeaderDetail" class="GvHeader">
                            <th class="hidden-xs filter">
                                Value Date
                            </th>
                            <th class="hidden-xs filter">
                                Deal Number
                            </th>
                            <th class="hidden-xs filter">
                                Acc. Number
                            </th>
                            <th class="hidden-xs filter">
                                Original Currency
                            </th>
                            <th class="hidden-xs filter">
                                Eq. USD
                            </th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                </table>
                <div class="pull-right">
                    <label>Total Eq. USD</label>
                    <input id="totalequsd" type="text" disabled="disabled">
                    <a id="BtnPrint" class="btn btn-primary" href="#" title="print"><i class="fa fa-print"></i> Print</a>
                    <label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</label>
                </div>
            </div>
        </div>
    </div>
    <style>
        .ui-datepicker-calendar {
        display: none;
    }
    </style>
    <script>
    var baseUrl = '@Url.Content("~")';
    var ajaxHandler = '@Html.Raw(Url.Action("DTDetailDealAjaxHandler", "InquiryLimitTrans", New With {.cif = ViewBag.cif, .accname = ViewBag.accname.ToString.Replace ("'","%22")}))'; @*'@Html.Action("DetailDealAjaxHandler", "InquiryLimitTrans", New With {.accnum = ViewBag.AccNum, .accname = ViewBag.AccName})';*@
    
        
    </script>
    @Scripts.Render("~/Scripts/datatables/detaildeal-index.js")
    <script>
    $(document).ready(function () {
        $('#filterDate').datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'mm-yy',
            onClose: function(dateText, inst) { 
                $(this).datepicker('setDate', new Date(inst.selectedYear, inst.selectedMonth, 1));
            }
            //dateFormat: "dd-mm-yy",
        });
    });
    $("#BtnPrint").click(function () {
        var cif = $("#txtCIF").val()
        var accname = $("#txtAccName").val();
        var period = $("#filterDate").val();

        if (period === undefined){
            period == ''
        }

        var link = 'RptDetailInquiryLimit?cif=' + encodeURIComponent(cif) + '&accname=' + encodeURIComponent(accname) + '&period=' + period;

        $(this).attr("href",link)
    })
    </script>
</body>
</html>