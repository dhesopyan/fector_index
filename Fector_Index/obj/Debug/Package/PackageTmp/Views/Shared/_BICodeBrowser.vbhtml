<html>
<head>
    <title>Browse BI Code</title>
</head>
<body>
    <div id="page-wrapper">
        <div class="row">
            <div class="col-md-12">
                <table id="myTable" class="table FectorTable">
                    <thead>
                        <tr id="GvFilter" class="GvHeader">
                            <th class="filter">
                                BI Code
                            </th>
                            <th class="filter">
                                Status
                            </th>
                            <th class="filter">
                                Name
                            </th>
                            <th>
                                &nbsp;
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
                                BI Code
                            </th>
                            <th>
                                Status
                            </th>
                            <th>
                                Name
                            </th>
                            <th>
                                Match (%)
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
        var ajaxHandler = '@Html.Raw(Url.Action("DTBICodeAjaxHandler", "BICode", New With {.name = ViewBag.Name.ToString.Replace ("'","%22"), .selectedstatus = ViewBag.SelectedStatus}))';
        function returnValue(e)
        {
            document.getElementById('BICode').value = e.trim();
            $('#modal-container').modal('hide');
        }
    </script>
    @Scripts.Render("~/Scripts/datatables/bicode-index.js")
</body>
</html>