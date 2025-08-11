<html>
<head>
    <title>Add Deal Number</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <table id="GvTransDetail" class="table FectorTable">
                    <thead>
                        <tr id="GvDetail" class="GvHeader">
                            <th class="filter">
                                Deal Number
                            </th>
                            <th class="hidden-xs filter">
                                Transaction Currency
                            </th>
                            <th class="hidden-xs filter">
                                Transaction Rate
                            </th>
                            <th class="hidden-xs filter">
                                Transaction Nominal
                            </th>
                            <th class="hidden-xs filter">
                                Customer Currency
                            </th>
                            <th class="hidden-xs filter">
                                Customer Rate
                            </th>
                            <th class="hidden-xs filter">
                                Customer Nominal
                            </th>
                            <th class="hidden-xs filter">
                                Value Period
                            </th>
                            <th class="hidden-xs filter">
                                Status
                            </th>
                            <th>&nbsp;</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </div>
    </div>
    <script>
        var baseUrl = '@Url.Content("~")';
        var ajaxHandler = '@Html.Raw(Url.Action("DTDealAjaxHandler", "ExchangeTransaction"))';
    </script>
    @Scripts.Render("~/Scripts/datatables/deal-in-transaction.js")
</body>
</html>