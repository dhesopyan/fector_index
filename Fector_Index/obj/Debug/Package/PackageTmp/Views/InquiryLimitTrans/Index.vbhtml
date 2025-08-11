@Code
    ViewData("Title") = "Inquiry Limit Transaction"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Inquiry Limit Transaction List</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="hidden-xs filter">
                                    CIF
                                </th>
                                <th class="hidden-xs filter">
                                    Account Name
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
                                <th class="hidden-xs filter">
                                    CIF
                                </th>
                                <th class="hidden-xs filter">
                                    Name
                                </th>
                                <th>&nbsp;</th>
                            </tr>
                        </thead>
                    </table>
                </div>
            </div>
        </div>
    </div>
    <div id="modal-container" class="modal fade" tabindex="-1" role="dialog">
        <div class="modal-content" id="modalpopup">
        </div>
    </div>
    <style>
        .dataTables_filter, .dataTables_info {
            display: none;
        }

        .modal-content {
            position: fixed;
            left: 0px;
            right: 0px;
            width: 800px !important;
            height: auto !important;
            margin: auto auto !important;
        }
    </style>

    @Section scripts
        <script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTLimitTransactionAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/inquirylimittrans-index.js")
    End Section
</div>

