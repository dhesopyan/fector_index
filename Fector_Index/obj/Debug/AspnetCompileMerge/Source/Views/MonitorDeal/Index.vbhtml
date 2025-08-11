@ModelType Fector_Index.CloseDealView 
@Code
    ViewData("Title") = "Monitor Deal"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
    Layout = "~/Views/Shared/_Layout.vbhtml"
End Code
<div class="col-md-12">
    <div class="panel panel-default">
        <div class="panel-heading">
            <div class="pull-left">
                <h4>Monitor Deal List</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        <div class="panel-body">
            <div class="row">
                <div class="col-md-12">
                    <table id="myTable" class="table FectorTable">
                        <thead>
                            <tr id="GvFilter" class="GvHeader">
                                <th class="filter">
                                    Deal Number
                                </th>
                                <th class="hidden">
                                    Reason
                                </th>
                                <th class="hidden-xs filter">
                                    Acc. Num.
                                </th>
                                <th class="hidden-xs filter">
                                    Acc. Name
                                </th>
                                <th class="hidden-xs filter">
                                    Branch
                                </th>
                                <th class="hidden-xs filter">
                                    Trans. Type
                                </th>
                                <th class="hidden-xs filter">
                                    Currency Deal
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th class="hidden-xs filter" data-id="datefilter">
                                    Value Date
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
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
                                <th class="filter">
                                    Deal Number
                                </th>
                                <th class="hidden">
                                    Reason
                                </th>
                                <th class="hidden-xs filter">
                                    Acc. Num.
                                </th>
                                <th class="hidden-xs filter">
                                    Acc. Name
                                </th>
                                <th class="hidden-xs filter">
                                    Branch
                                </th>
                                <th class="hidden-xs filter">
                                    Trans. Type
                                </th>
                                <th class="hidden-xs filter">
                                    Currency Deal
                                </th>
                                <th class="hidden-xs filter">
                                    Amount Customer
                                </th>
                                <th class="hidden-xs filter">
                                    Value Date
                                </th>
                                <th class="hidden-xs filter">
                                    Amount Deal
                                </th>
                                <th class="hidden-xs filter">
                                    Remain. Deal
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
    </div>
</div>
@Using Html.BeginForm("CloseDeal", "MonitorDeal", FormMethod.Post, New With {.class = "panel panel-default", .id = "frmData"})
    @<div id="Modal" class="modal fade" tabindex="-1" role="dialog">
    <div class="modal-dialog">
        <div class="modal-content">
            <div class="modal-header">
                <button type="button" class="close" data-dismiss="modal">&times;</button>
                <h4 class="modal-title">Close Deal</h4>
            </div>
            <div class="modal-body" style="height: 200px;">
                <div class="col-md-12 ">
                    <label>@Html.DisplayNameFor(Function(f) f.DealNumber)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DealNumber, New With {.class = "form-control", .placeholder = "Deal Number", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.DealNumber, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="col-md-12 ">
                    <label>@Html.DisplayNameFor(Function(f) f.CloseDealReason)</label>
                    @Html.CustomTextBoxFor(Function(f) f.CloseDealReason, New With {.class = "form-control", .placeholder = "Close Deal Reason", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.CloseDealReason, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="modal-footer">
                <button class="btn btn-primary" id="btnSave" value="Save"><i class="fa fa-check"></i> Save</button>
                <button class="btn btn-danger" id="btnCancel" data-dismiss="modal" value="Cancel"><i class="fa fa-close"></i> Cancel</button>
            </div>
        </div>
    </div>
</div>
End Using

<style>
    .dataTables_filter, .dataTables_info {
        display: none;
    }
    .modal-content {
         position: fixed;
         left: 0px;
         right: 0px;
         width: 650px !important;
         height: auto !important;
         margin: auto auto !important;
     }
</style>
@Section scripts
    <script>
        var baseUrl = '@Url.Content("~")';
        var ajaxHandler = '@Url.Action("DTMonitorDealAjaxHandler")'; 

        $(document).ready(function () {
            $('#DealNumber').attr('readonly', 'readonly');
        });

        $('#Modal').on('hidden.bs.modal', function (e) {
            $("#DealNumber").val("");
            $("#CloseDealReason").val("");
        });
    </script>
    @Scripts.Render("~/Scripts/datatables/monitordeal-index.js")

<script>
    $(document).ready(function () {
        $('#datefilter').datepicker({
            dateFormat: "dd-mm-yy",
        });
    });
</script>
End Section
