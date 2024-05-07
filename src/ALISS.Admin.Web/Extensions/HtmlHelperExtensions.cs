using Ganss.Xss;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ALISS.Admin.Web.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Create a standard text box for a model property.
        /// The icon text is set using the Display Description Attribute
        /// The label text is set using the Display Name Attribute, or the Property Name Attribue
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static IHtmlString ALISSTextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string type = "text", bool includeWordCount = false, bool descriptionAsText = false, IHtmlString checkbox = null)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", metaData.PropertyName);

            if (!String.IsNullOrEmpty(metaData.Description) && !descriptionAsText)
            {
                TagBuilder icon = new TagBuilder("i");
                icon.AddCssClass("fas fa-info-circle");
                icon.MergeAttribute("aria-hidden", "true");
                icon.MergeAttribute("data-tip", metaData.Description);
                label.InnerHtml = $"{labelText} {icon.ToString(TagRenderMode.Normal)}";
            }
            else
            {
                label.InnerHtml = labelText;
            }

            TagBuilder description = new TagBuilder("span");
            if (!string.IsNullOrEmpty(metaData.Description) && descriptionAsText)
            {
                TagBuilder br = new TagBuilder("br");
                description.InnerHtml = $"{metaData.Description}{br.ToString(TagRenderMode.SelfClosing)}{br.ToString(TagRenderMode.SelfClosing)}";
            }

            TagBuilder input = new TagBuilder("input");
            input.AddCssClass("aliss-form__input");
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("type", type);
            input.MergeAttribute("value", metaData.Model == null ? "" : metaData.Model.ToString());

            TagBuilder count = new TagBuilder("span");
            count.MergeAttribute("id", metaData.PropertyName + "_Count");
            var maxLength = GetMaximumLength(metaData);
            string script = "";
            if (maxLength != null)
            {
                count.InnerHtml = $"{int.Parse(maxLength) - (metaData.Model?.ToString().Length ?? 0)} character(s) remaining";

                script = $@"
                    <script>
                        document.getElementById(""{metaData.PropertyName}"").onkeyup = function () {{
                            var maxLength = {maxLength};
                            var fieldLength = document.getElementById(""{metaData.PropertyName}"").value.length;
                            document.getElementById(""{metaData.PropertyName + "_Count"}"").innerText = (maxLength - fieldLength) + "" character(s) remaining"";
                        }};
                    </script>
                ";
            }

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            if (checkbox != null)
            {
                htmlBuilder.Append(checkbox);
            }
            if (!string.IsNullOrWhiteSpace(description.InnerHtml))
            {
                htmlBuilder.Append(description.ToString(TagRenderMode.Normal));
            }
            htmlBuilder.Append(input.ToString(TagRenderMode.SelfClosing));
            if (includeWordCount)
            {
                htmlBuilder.Append(count.ToString(TagRenderMode.Normal));
                htmlBuilder.Append(script);
            }
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSTextArea<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string placeholderText, string extraClass = "", int rows = 0, bool includeWordCount = false, bool descriptionAsText = false)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", metaData.PropertyName);

            if (!String.IsNullOrEmpty(metaData.Description) && !descriptionAsText)
            {
            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass("fas fa-info-circle");
            icon.MergeAttribute("aria-hidden", "true");
            icon.MergeAttribute("data-tip", metaData.Description);
            label.InnerHtml = $"{labelText} {icon.ToString(TagRenderMode.Normal)}";
            }
            else
            {
                label.InnerHtml = labelText;
            }

            TagBuilder description = new TagBuilder("span");
            if (!string.IsNullOrEmpty(metaData.Description) && descriptionAsText)
            {
                TagBuilder br = new TagBuilder("br");
                description.InnerHtml = $"{metaData.Description}{br.ToString(TagRenderMode.SelfClosing)}{br.ToString(TagRenderMode.SelfClosing)}";
            }

            TagBuilder textArea = new TagBuilder("textarea");
            textArea.AddCssClass("aliss-form__textarea");
            textArea.AddCssClass(extraClass);
            textArea.MergeAttribute("id", metaData.PropertyName);
            textArea.MergeAttribute("name", metaData.PropertyName);
            textArea.MergeAttribute("placeholder", placeholderText);
            if (rows > 0)
            {
                textArea.MergeAttribute("rows", rows.ToString());
            }
            textArea.InnerHtml = metaData.Model == null ? "" : metaData.Model.ToString();

            TagBuilder count = new TagBuilder("span");
            count.MergeAttribute("id", metaData.PropertyName + "_Count");
            var maxLength = GetMaximumLength(metaData);
            string script = "";
            if (maxLength != null)
            {
                count.InnerHtml = $"{int.Parse(maxLength) - (metaData.Model?.ToString().Length ?? 0)} character(s) remaining";

                script = $@"
                    <script>
                        document.getElementById(""{metaData.PropertyName}"").onkeyup = function () {{
                            var maxLength = {maxLength};
                            var fieldLength = document.getElementById(""{metaData.PropertyName}"").value.length;
                            document.getElementById(""{metaData.PropertyName + "_Count"}"").innerText = (maxLength - fieldLength) + "" character(s) remaining"";
                        }};
                    </script>
                ";
            }

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            if (!string.IsNullOrWhiteSpace(description.InnerHtml))
            {
                htmlBuilder.Append(description.ToString(TagRenderMode.Normal));
            }
            htmlBuilder.Append(textArea.ToString(TagRenderMode.Normal));
            if (includeWordCount)
            {
                htmlBuilder.Append(count.ToString(TagRenderMode.Normal));
                htmlBuilder.Append(script);
            }
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSRTEditor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string placeholderText, int maxLength = 0, bool includeWordCount = false, bool descriptionAsText = false, bool fullToolbar = false, IHtmlString checkbox = null)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", metaData.PropertyName);

            if (!String.IsNullOrEmpty(metaData.Description) && !descriptionAsText)
            {
                TagBuilder icon = new TagBuilder("i");
                icon.AddCssClass("fas fa-info-circle");
                icon.MergeAttribute("aria-hidden", "true");
                icon.MergeAttribute("data-tip", metaData.Description);
                label.InnerHtml = $"{labelText} {icon.ToString(TagRenderMode.Normal)}";
            }
            else
            {
            label.InnerHtml = labelText;
            }

            TagBuilder description = new TagBuilder("span");
            if (!string.IsNullOrEmpty(metaData.Description) && descriptionAsText)
            {
                TagBuilder br = new TagBuilder("br");
                description.InnerHtml = $"{metaData.Description}{br.ToString(TagRenderMode.SelfClosing)}{br.ToString(TagRenderMode.SelfClosing)}";
            }

            TagBuilder editorDiv = new TagBuilder("div");
            editorDiv.MergeAttribute("id", "editorDiv");

            TagBuilder editor = new TagBuilder("div");
            editor.AddCssClass("aliss-form__textarea");
            editor.MergeAttribute("id", "editor");
            editor.InnerHtml = metaData.Model?.ToString();

            editorDiv.InnerHtml = editor.ToString(TagRenderMode.Normal);

            TagBuilder hiddenInput = new TagBuilder("input");
            hiddenInput.AddCssClass("aliss-form__input");
            hiddenInput.MergeAttribute("id", metaData.PropertyName);
            hiddenInput.MergeAttribute("name", metaData.PropertyName);
            hiddenInput.MergeAttribute("type", "hidden");
            hiddenInput.MergeAttribute("value", metaData.Model == null ? "" : metaData.Model.ToString());

            TagBuilder count = new TagBuilder("span");
            count.MergeAttribute("id", metaData.PropertyName + "_Count");
            string countScript = "";

            if (maxLength > 0)
            {
                count.InnerHtml = $"{maxLength - (SanitizeHtmlString(metaData.Model?.ToString()).Length)} character(s) remaining";

                if (includeWordCount)
                {
                    countScript = $@"
                        var editor = document.getElementsByClassName(""ql-editor"")[0];
                        editor.onkeyup = function () {{
                            var maxLength = {maxLength};
                            var fieldLength = editor.innerText.replaceAll(""\n"", """").length;
                            document.getElementById(""{metaData.PropertyName + "_Count"}"").innerText = (maxLength - fieldLength) + "" character(s) remaining"";
                        }};";
                }
            }

            var script = @"<script>
                var toolbarOptions = [";
            if (fullToolbar)
            {
                script += @" 
                    ['bold', 'italic', 'underline', 'strike'],
                    ['link'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }],
                    [{ 'script': 'sub'}, { 'script': 'super' }],
                    [{ 'indent': '-1'}, { 'indent': '+1' }],
                    [{ 'header': [1, 2, 3, 4, 5, 6, false] }],
                    [{ 'color': [] }, { 'background': [] }]"
;
            }
            else
            {
                script += @"
                    ['bold'],
                    [{ 'list': 'bullet' }],
                    [{ 'indent': '-1'}, { 'indent': '+1' }],";
            }

            script += @"
                ];
                    
                var options = {
                    modules: {
                        toolbar: toolbarOptions
                    },
                    placeholder: '" + placeholderText + @"',
                    theme: 'snow'
                };
                var rteditor = new Quill('#editor', options);

                rteditor.clipboard.addMatcher (Node.ELEMENT_NODE, function (node, delta) {
                    var plaintext = node.innerText
                    var Delta = Quill.import('delta')
                    return new Delta().insert(plaintext)
                });

                function handleSubmit()
                {
                    var rteInput = rteditor.root.innerText;
                    if (rteInput.trim().length < 1) {
                        document.getElementById('" + metaData.PropertyName + @"').value = '';
                    } else {
                        document.getElementById('" + metaData.PropertyName + @"').value = rteditor.root.innerHTML;
                    }
                }
                " + countScript + @"
            </script>";

        StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            if (checkbox != null)
            {
                htmlBuilder.Append(checkbox);
            }
            if (!string.IsNullOrWhiteSpace(description.InnerHtml))
            {
                htmlBuilder.Append(description.ToString(TagRenderMode.Normal));
            }
            htmlBuilder.Append(editorDiv.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(hiddenInput.ToString(TagRenderMode.Normal));
            if (includeWordCount)
            {
                htmlBuilder.Append(count.ToString(TagRenderMode.Normal));
            }
            htmlBuilder.Append(script);
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSCheckbox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group aliss-form__group--checkbox");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder innerDiv = new TagBuilder("div");
            innerDiv.AddCssClass("aliss-form__checkbox");

            TagBuilder input = new TagBuilder("input");
            input.AddCssClass("aliss-form__input");
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("type", "checkbox");
            input.MergeAttribute("value", "true");
            if (metaData.Model != null && metaData.Model.ToString() == "True")
            {
                input.MergeAttribute("checked", "");
            }

            TagBuilder label = new TagBuilder("label");
            label.MergeAttribute("for", metaData.PropertyName);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<span>{labelText}</span>");
            if (!String.IsNullOrEmpty(metaData.Description))
            {
                stringBuilder.Append($"<br />{metaData.Description}");
            }
            label.InnerHtml = stringBuilder.ToString();

            StringBuilder htmlString = new StringBuilder();
            htmlString.Append(input.ToString(TagRenderMode.SelfClosing));
            htmlString.Append(label.ToString(TagRenderMode.Normal));
            innerDiv.InnerHtml = htmlString.ToString();
            outerDiv.InnerHtml = innerDiv.ToString(TagRenderMode.Normal);
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSInnerCheckbox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, bool removeUpperMargin = true)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>");

            TagBuilder innerDiv = new TagBuilder("div");
            if (removeUpperMargin)
            {
                innerDiv.MergeAttribute("style", "margin-top: -20px;");
            }
            innerDiv.AddCssClass("aliss-form__checkbox");

            TagBuilder input = new TagBuilder("input");
            input.AddCssClass("aliss-form__input");
            input.MergeAttribute("id", metaData.PropertyName);
            input.MergeAttribute("name", metaData.PropertyName);
            input.MergeAttribute("type", "checkbox");
            input.MergeAttribute("value", "true");
            if (metaData.Model != null && metaData.Model.ToString() == "True")
            {
                input.MergeAttribute("checked", "");
            }

            TagBuilder label = new TagBuilder("label");
            label.MergeAttribute("for", metaData.PropertyName);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"<p>{labelText}</p>");
            if (!String.IsNullOrEmpty(metaData.Description))
            {
                stringBuilder.Append($"<br />{metaData.Description}");
            }
            label.InnerHtml = stringBuilder.ToString();

            StringBuilder htmlString = new StringBuilder();
            htmlString.Append(input.ToString(TagRenderMode.SelfClosing));
            htmlString.Append(label.ToString(TagRenderMode.Normal));
            innerDiv.InnerHtml = htmlString.ToString();
            string html = innerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        /*public static IHtmlString ALISSRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        { 
            <div class="aliss-form__radio">
                @Html.RadioButtonFor(m => m.OrganisationRepresentative, true, new { id = "OrganisationRepresentative-Yes" })
                @Html.Label("OrganisationRepresentative-Yes", "Yes")
            </div>
        }*/

        public static IHtmlString ALISSDisabledTextBox<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", $"{metaData.PropertyName}Display");

            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass("fas fa-info-circle");
            icon.MergeAttribute("aria-hidden", "true");
            icon.MergeAttribute("data-tip", metaData.Description);
            label.InnerHtml = $"{labelText} {icon.ToString(TagRenderMode.Normal)}";

            TagBuilder input = new TagBuilder("input");
            input.AddCssClass("aliss-form__input");
            input.MergeAttribute("id", $"{metaData.PropertyName}Display");
            input.MergeAttribute("disabled", "true");
            input.MergeAttribute("type", "text");
            input.MergeAttribute("value", metaData.Model == null ? "" : metaData.Model.ToString());

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(input.ToString(TagRenderMode.SelfClosing));
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSDropDown<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, List<SelectListItem> options = null)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); ;
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", metaData.PropertyName);

            TagBuilder icon = new TagBuilder("i");
            icon.AddCssClass("fas fa-info-circle");
            icon.MergeAttribute("aria-hidden", "true");
            icon.MergeAttribute("data-tip", metaData.Description);
            label.InnerHtml = $"{labelText} {icon.ToString(TagRenderMode.Normal)}";

            TagBuilder select = new TagBuilder("select");
            select.AddCssClass(options == null ? "aliss-form__icon" : "aliss-form__select");
            select.MergeAttribute("id", metaData.PropertyName);
            select.MergeAttribute("name", metaData.PropertyName);
            if (options == null)
            {
                select.MergeAttribute("data-icon-selected", metaData.Model == null ? "" : metaData.Model.ToString());
            }
            else
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (SelectListItem option in options)
                {
                    stringBuilder.AppendFormat("<option value='{0}' {2}>{1}</option>", option.Value, option.Text, (option.Selected ? "selected" : ""));
                }
                select.InnerHtml = stringBuilder.ToString();
            }

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(select.ToString(TagRenderMode.Normal));
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        public static IHtmlString ALISSDisplay<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression<TModel, TProperty>(expression, htmlHelper.ViewData);
            string labelText = metaData.DisplayName ?? metaData.PropertyName;
            string validationMessage = htmlHelper.ValidationMessageFor(expression).ToHtmlString().Replace(metaData.PropertyName, labelText);
            bool hasError = !String.IsNullOrEmpty(validationMessage) && !validationMessage.Contains("\"></span>"); 
            TagBuilder outerDiv = new TagBuilder("div");
            outerDiv.AddCssClass("aliss-form__group");
            if (hasError)
            {
                outerDiv.AddCssClass("aliss-form__group--error");
            }

            TagBuilder label = new TagBuilder("label");
            label.AddCssClass("aliss-form__label");
            label.MergeAttribute("for", metaData.PropertyName);

            label.InnerHtml = labelText;

            TagBuilder input = new TagBuilder("span");
            input.InnerHtml = metaData.Model == null ? "" : metaData.Model.ToString();

            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append(label.ToString(TagRenderMode.Normal));
            htmlBuilder.Append(input.ToString(TagRenderMode.Normal));
            outerDiv.InnerHtml = htmlBuilder.ToString();
            string html = outerDiv.ToString(TagRenderMode.Normal);

            return MvcHtmlString.Create(html);
        }

        private static string GetMaximumLength(ModelMetadata metaData)
        {
            string maxLength = null;

            // Get the MaxLengthAttribute from the property's metadata
            if (metaData.ContainerType.GetProperty(metaData.PropertyName)
                .GetCustomAttributes(typeof(MaxLengthAttribute), false)
                .FirstOrDefault() is MaxLengthAttribute maxLengthAttribute)
            {
                maxLength = maxLengthAttribute.Length.ToString();
            }

            return maxLength;
        }

        private static string SanitizeHtmlString(string html)
        {
            HtmlSanitizer _lengthSanitizer = new HtmlSanitizer();

            _lengthSanitizer.AllowedTags.Clear();
            _lengthSanitizer.AllowedAtRules.Clear();
            _lengthSanitizer.AllowedAttributes.Clear();
            _lengthSanitizer.AllowedClasses.Clear();
            _lengthSanitizer.AllowedCssProperties.Clear();
            _lengthSanitizer.AllowedSchemes.Clear();
            _lengthSanitizer.AllowDataAttributes = false;
            _lengthSanitizer.KeepChildNodes = true;

            return _lengthSanitizer.Sanitize(html);
        }
    }
}