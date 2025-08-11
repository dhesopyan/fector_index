<html>
<head>
    <title>Browse Source Account Number</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <table id="myTable" class="table FectorTable">
                    <thead>
                        <tr id="GvFilter" class="GvHeader">
                            <th class="filter">
                                Account Number
                            </th>
                            <th class="filter">
                                Name
                            </th>
                            <th>
                                <a id="BtnFilter" class="btn btn-primary btn-sm">
                                    <span class="fa fa-search"></span>
                                </a>
                                <a id="BtnClear" class="btn btn-primary btn-sm">
                                    <span class="fa fa-refresh"></span>
                                </a>
                            </th>
                        </tr>
                        <tr id="GvHeader" class="GvHeader">
                            <th>
                                Account Number
                            </th>
                            <th>
                                Name
                            </th>
                            <th>
                                &nbsp; 
                            </th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
    <script>
        var baseUrl = '@Url.Content("~")';
        var ajaxHandler = '@Html.Raw(Url.Action("DTAccountAjaxHandler", "TransactionDeal"))';
        function returnValue(AccNo,AccName,TOMLimit,SPOTLimit)
        {
            $('#AccNum').change();
            $('#AccName').change();

            document.getElementById('AccNum').value = AccNo;
            document.getElementById('AccName').value = AccName.replace("%22","'");

            var markup = "";
            if (TOMLimit > 0 && SPOTLimit > 0){
                markup += "<option value='TOD'>TOD</option>";
                markup += "<option value='TOM'>TOM</option>";
                markup += "<option value='SPOT'>SPOT</option>";
                markup += "<option value='FWD'>FORWARD</option>";
            }
            else {
                if (TOMLimit > 0){
                    markup += "<option value='TOD'>TOD</option>";
                    markup += "<option value='TOM'>TOM</option>";
                    markup += "<option value='FWD'>FORWARD</option>";
                }
                else if(SPOTLimit > 0){
                    markup += "<option value='TOD'>TOD</option>";
                    markup += "<option value='SPOT'>SPOT</option>";
                    markup += "<option value='FWD'>FORWARD</option>";
                }
                else {
                    markup += "<option value='TOD'>TOD</option>";
                    markup += "<option value='FWD'>FORWARD</option>";
                }
            }

            $("#DealPeriod").html(markup).show();
            $("#DealPeriod").select2({ width: '100%' });
            $('#modal-container').modal('hide');
        }
    </script>
    @Scripts.Render("~/Scripts/datatables/accnum-index.js")
</body>
</html>