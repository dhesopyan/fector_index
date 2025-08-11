<html>
<head>
    <title>Browse Source Account Number</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <table id="myTable2" class="table FectorTable">
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
        function returnVal(AccNo,AccName)
        {
            $('#SourceAccNum').change();
            $('#SourceAccName').change();

            document.getElementById('SourceAccNum').value = AccNo;
            document.getElementById('SourceAccName').value = AccName.replace("%22","'");
            $('#modal-container2').modal('hide');
        }
    </script>
    @Scripts.Render("~/Scripts/datatables/sourceaccnum-index.js")
</body>
</html>