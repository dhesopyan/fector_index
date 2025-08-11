@ModelType Fector_Index.CoreNonTrxViewModel
@Code
    ViewData("Title") = "Index"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code

@Using Html.BeginForm("Index", "CoreNonTrx", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData"})
    @Html.AntiForgeryToken()
    @<div class="col-md-12">
        <div class="panel panel-default">
            <div class="panel-heading">
                <div class="pull-left">
                    <h4>Core Non-Transaction List</h4>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="panel-body">
                <div class="row">
                    <div class="col-md-12">
                        <table id="myTable" class="table FectorTable">
                            <thead>
                                <tr id="GvFilter" class="GvHeader">
                                    <th class="filter hidden">
                                        RefNum
                                    </th>
                                    <th class="hidden-xs filter">
                                        TDate
                                    </th>
                                    <th class="hidden-xs filter">
                                        Acc. Number
                                    </th>
                                    <th class="hidden-xs filter">
                                        Acc. Name
                                    </th>
                                    <th class="hidden-xs filter">
                                        Currency
                                    </th>
                                    <th class="hidden-xs filter">
                                        Branch
                                    </th>
                                    <th class="hidden-xs filter">
                                        Amount
                                    </th>
                                    <th class="hidden-xs filter">
                                        Time
                                    </th>
                                    <th class="hidden-xs filter">
                                        Rate
                                    </th>
                                    <th class="hidden-xs filter" id="filterDDL">
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
                                <tr id="GvHeader" class="GvHeader">
                                    <th class="filter hidden">
                                        RefNum
                                    </th>
                                    <th class="hidden-xs filter">
                                        TDate
                                    </th>
                                    <th class="hidden-xs filter">
                                        Acc. Number
                                    </th>
                                    <th class="hidden-xs filter">
                                        Acc. Name
                                    </th>
                                    <th class="hidden-xs filter">
                                        Currency
                                    </th>
                                    <th class="hidden-xs filter">
                                        Branch
                                    </th>
                                    <th class="hidden-xs filter">
                                        Amount
                                    </th>
                                    <th class="hidden-xs filter">
                                        Time
                                    </th>
                                    <th class="hidden-xs filter">
                                        Rate
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
            <div class="panel-footer">
                <div class="pull-right">
                    <button name="button" value="BtnProcess" class="btn btn-primary"><i class="fa fa-cogs"></i> Process</button>
                    <button name="button" value="BtnDelete" class="btn btn-primary"><i class="fa fa-trash"></i> Delete</button>
                    <button name="button" value="BtnActive" class="btn btn-primary"><i class="fa fa-check"></i> Active</button>
                    <button name="button" value="BtnInactive" class="btn btn-primary"><i class="fa fa-close"></i> Inactive</button>
                </div>
                <div class="clearfix"></div>
            </div>
        </div>
    </div>
    @<style>
         .dataTables_filter, .dataTables_info {
             display: none;
         }
    </style>
    @Section scripts
        <script>
            var baseUrl = '@Url.Content("~")';
            var ajaxHandler = '@Url.Action("DTCoreNonTrxAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/corenontrx-index.js")
    End Section
End Using
