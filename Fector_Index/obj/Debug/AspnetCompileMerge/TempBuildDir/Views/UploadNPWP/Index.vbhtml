@ModelType Fector_Index.MsUploadNPWPView

@Code
    ViewData("Title") = "Upload NPWP"
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
@Using Html.BeginForm("Index", "UploadNPWP", FormMethod.Post, New With {.id = "frm", .enctype = "multipart/form-data"})
    @<div class="col-md-12">
        <div class="panel panel-default">
            <div class="panel-heading">
                <div class="pull-left">
                    <h4>Account Non NPWP List</h4>
                </div>
                <div class="clearfix"></div>
            </div>
            <div class="panel-body">
                <div class="row hidden">
                    <div class="col-md-12 ">
                        <input type="text" id="accno" name="accno">@Html.DisplayFor(Function(f) f.accno)
                        <input type="text" id="lastfiledirectory" name="lastfiledirectory">@Html.DisplayFor(Function(f) f.LastFileDirectory )
                        <label for="file">@Html.DisplayFor(Function(f) f.File)</label>
                        <input type="file" id="File" name="File" />
                    </div>
                </div>
                <div class="row">
                    <div class="col-md-12">
                        <table id="myTable" class="table FectorTable">
                            <thead>
                                <tr id="GvFilter" class="GvHeader">
                                    <th class="filter">
                                        Account Number
                                    </th>
                                    <th class="filter">
                                        Account Name
                                    </th>
                                    <th class="filter">
                                        CIF
                                    </th>
                                    <th class="filter" id="filterDDL">
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
                                    <th>
                                        Account Number
                                    </th>
                                    <th>
                                        CIF
                                    </th>
                                    <th>
                                        Account Name
                                    </th>
                                    <th>
                                        Status
                                    </th>
                                    <th>&nbsp;</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
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
            var ajaxHandler = '@Url.Action("DTAccountNonNPWPAjaxHandler")';
        </script>
        @Scripts.Render("~/Scripts/datatables/accountnonnpwp-index.js")
    End Section
End Using


