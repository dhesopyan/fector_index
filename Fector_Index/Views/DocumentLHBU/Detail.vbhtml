@ModelType Fector_Index.MsDocumentLHBU 
@Code
    Layout = "~/Views/Shared/_Layout.vbhtml"
    Dim Action As String = ViewContext.Controller.ValueProvider.GetValue("Action").RawValue
End Code

<div class="col-md-12">
    @Using Html.BeginForm(Action, "DocumentLHBU", FormMethod.Post, New With {.class = "panel panel-default", .defaultbutton = "BtnSubmit"})
        @Html.AntiForgeryToken()
        @<div class="panel-heading">
            <div class="pull-left">
                <h4>Document LHBU Information</h4>
            </div>
            <div class="clearfix"></div>
        </div>
        @<div class="panel-body">
            <div class="row">
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.DocumentId)</label>
                    @Html.CustomTextBoxFor(Function(f) f.DocumentId, New With {.class = "form-control", .placeholder = "Document ID", .style = "text-transform:uppercase"})
                    @Html.ValidationMessageFor(Function(f) f.DocumentId, String.Empty, New With {.class = "help-block"})
                </div>
                <div class="form-group col-md-6">
                    <label>@Html.DisplayNameFor(Function(f) f.Description)</label>
                    @Html.CustomTextBoxFor(Function(f) f.Description, New With {.class = "form-control", .placeholder = "Description"})
                    @Html.ValidationMessageFor(Function(f) f.Description, String.Empty, New With {.class = "help-block"})
                </div>
            </div>
        </div>
        @<div class="panel-footer">
            <div class="pull-right">
                <button id="BtnSubmit" class="btn btn-primary"><i class="fa fa-save"></i> Save</button>
                <a id="BtnBack" class="btn btn-danger" href="@Url.Action("Index", "DocumentLHBU")"><i class="fa fa-arrow-left"></i> Back</a>
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
        });
        </script>
    End Section
</div>
