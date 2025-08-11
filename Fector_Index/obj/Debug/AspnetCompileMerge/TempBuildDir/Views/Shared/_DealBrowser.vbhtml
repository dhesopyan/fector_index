<html>
<head>
    <title>Browse Deal</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <table id="myTable" class="table FectorTable">
                    <thead>
                        <tr id="GvHeader" class="GvHeader">
                            <th class="hidden filter">
                                ID
                            </th>
                            <th class="hidden-xs filter">
                                Deal Number
                            </th>
                            <th class="hidden-xs filter">
                                Acc. Num.
                            </th>
                            <th class="hidden-xs filter">
                                Acc. Name
                            </th>
                            <th class="hidden-xs filter">
                                Trans. Type
                            </th>
                            <th class="hidden-xs filter">
                                Currency Deal
                            </th>
                            <th class="hidden-xs filter" >
                                Value Date
                            </th>
                            <th class="hidden-xs filter hidden">
                                Status
                            </th>
                            <th>&nbsp;</th>
                        </tr>
                        <tr id="GvFilter" class="GvHeader">
                            <th class="filter hidden">
                                ID
                            </th>
                            <th class="filter">
                                Deal Number
                            </th>
                            <th class="filter">
                                Acc. Num.
                            </th>
                            <th class="filter">
                                Acc. Name
                            </th>
                            <th class="filter">
                                Trans. Type
                            </th>
                            <th class="filter">
                                Currency Deal
                            </th>
                            <th class="filter" data-id="datefilter">
                                Value Date
                            </th>
                            <th class="hidden-xs filter hidden" id="filterDDL">
                                Status
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
                    </thead>
                </table>
            </div>
        </div>
    </div>
    <script>
        var baseUrl = '@Url.Content("~")';
        var ajaxHandler = '@Html.Raw(Url.Action("DTDealAjaxHandler", "TransactionDeal"))';
        function returnValue(AccNo,AccName,Branch,TransactionType,DealType,CurrencyDeal,AmountDeal,DealRate,CurrencyCustomer,AmountCustomer,RateCustomer,DealPeriod,DealDate)
        {
            document.getElementById('AccNum').value = AccNo;
            document.getElementById('AccName').value = AccName.replace("%22", "'");
            $('#BranchId').select2().select2('val', Branch);
            $('#TransactionType').select2().select2('val', TransactionType);
            $('#DealType').select2().select2('val', DealType);
            $('#CurrencyDeal').select2().select2('val', CurrencyDeal);
            document.getElementById('AmountDeal').value = AmountDeal;
            document.getElementById('DealRate').value = DealRate;
            $('#CurrencyCustomer').select2().select2('val', CurrencyCustomer);
            document.getElementById('AmountCustomer').value = AmountCustomer;
            document.getElementById('RateCustomer').value = RateCustomer;
            $('#modal-container').modal('hide');
        }

        $(document).ready(function () {
            $('#datefilter').datepicker({
                dateFormat: "dd-mm-yy",
            });
        });
    </script>
    @Scripts.Render("~/Scripts/datatables/deal-index.js")

    <script>
        $(document).ready(function () {
            $('#datefilter').datepicker({
                dateFormat: "dd-mm-yy",
            });
        });
    </script>
</body>
</html>