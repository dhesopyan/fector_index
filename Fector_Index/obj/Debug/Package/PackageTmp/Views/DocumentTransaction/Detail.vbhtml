@ModelType Fector_Index.MsDocumentTransactionViewModel 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim DocumentTypes As IEnumerable(Of SelectListItem) = ViewBag.DocumentTypes
    Dim CustomerTypes As IEnumerable(Of SelectListItem) = ViewBag.CustomerTypes
    Dim LHBUPurposes As MultiSelectList = ViewBag.LHBUPurposes
    Dim LHBUDocuments As MultiSelectList = ViewBag.LHBUDocuments
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "DocumentTransaction", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Document Underlying Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-12">
                    @Html.HiddenFor(Function(f) f.DocumentId, New With {.class = "form-control", .placeholder = "Document ID", .style = "text-transform:uppercase"})
                    <label>@Html.DisplayNameFor(Function(f) f.Description)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Description, New With {.class = "form-control", .placeholder = "Description"})
                    @Html.ValidationMessageFor(Function(f) f.Description, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentType)</label>
                    @Html.DropDownListFor(Function(f) f.DocumentType, DocumentTypes, New With {.class = "form-control", .placeholder = "Document Type", .id="DocTypes"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentType, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.CustomerType)</label>
                    @Html.DropDownListFor(Function(f) f.CustomerType, CustomerTypes, New With {.class = "form-control", .placeholder = "Customer Type"})
                    @Html.ValidationMessageFor(Function(f) f.CustomerType, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
             <div class="row">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.SelectedLHBUPurposes)</label>
                     @Html.ListBoxFor(Function(f) f.SelectedLHBUPurposes, LHBUPurposes, New With {.class = "form-control", .placeholder = "LHBU Purposes", .multiple = "multiple"})
                     @Html.ValidationMessageFor(Function(f) f.SelectedLHBUPurposes, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
             <div class="row">
                 <div class="form-group col-md-12">
                     <label>@Html.DisplayNameFor(Function(f) f.SelectedLHBUDocuments)</label>
                     @Html.ListBoxFor(Function(f) f.SelectedLHBUDocuments, LHBUDocuments, New With {.class = "form-control", .placeholder = "LHBU Documents", .multiple="multiple"})
                     @Html.ValidationMessageFor(Function(f) f.SelectedLHBUDocuments, String.Empty, New With {.class = "help-block"})
                 </div>
             </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                @If (Action = "Edit" or Action = "Create") Then
                    @<button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                ElseIf (Action = "Process") Then
                    @<button name="approvalButton" class="btn btn-primary" value="Approve"><i class="fa fa-check"></i> Approve</button>
                    @<button name="approvalButton" class="btn btn-primary" value="Reject"><i class="fa fa-close"></i> Reject</button>
                End If
                @If (Action = "Process") Then
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Approval", "DocumentTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                Else
                    @<a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "DocumentTransaction")"><i class="fa fa-arrow-left"></i> Back</a>
                End If
                
            </div>
            <div class="clearfix"></div>
        </div>
        End Using
    @Section scripts
        <script>
        $(document).ready(function(){
            if('@Action' == 'Edit')
            {
                $('#DocumentId').attr('readonly','readonly');
            }
            else if ('@Action' == 'Process') {
                $('#DocumentId').attr('readonly', 'readonly');
                $('#Description').attr('readonly', 'readonly');
                DocTypes.disabled = true;
                CustomerType.disabled = true;
                SelectedLHBUPurposes.disabled = true;
                SelectedLHBUDocuments.disabled = true;
            }
        });
        </script>
    End Section
</div>
