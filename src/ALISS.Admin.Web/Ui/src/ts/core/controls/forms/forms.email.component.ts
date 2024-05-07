import Aliss from '../../';

class Email {
    constructor () {
        enum ID {
            InputSection = "emailInput",
            InputSubject = "Subject",
            InputBody = "editor",
            InputCC = "CCAlissAdmin",
            InputImage = "EmailAttachment",

            PreviewSection = "previewEmail",
            PreviewSubject = "previewSubject",
            PreviewCC = "previewCC",
            PreviewImage = "previewImage",
            
            EmailTitle = "customEmailTitle",
            EmailSubject = "customEmailSubject",
            EmailBody = "customEmailBody",

            PreviewButton = "checkbox-emailPreview",

            ErrorMessage = "error"
        }

        const init = () => {

        }

        init();

        var input = document.getElementById(ID.InputSection) as HTMLDivElement;
        var preview = document.getElementById(ID.PreviewSection) as HTMLDivElement;
        var previewButton = document.getElementById(ID.PreviewButton) as HTMLInputElement;
        var previewSubject = document.getElementById(ID.PreviewSubject) as HTMLLabelElement;
        var previewCC = document.getElementById(ID.PreviewCC) as HTMLLabelElement;
        var previewImage = document.getElementById(ID.PreviewImage) as HTMLLabelElement;
        var inputSubject = document.getElementById(ID.InputSubject) as HTMLInputElement;
        var inputBody = document.getElementById(ID.InputBody)?.firstChild as HTMLInputElement;
        var inputCC = document.getElementById(ID.InputCC) as HTMLInputElement;
        var inputImage = document.getElementById(ID.InputImage) as HTMLInputElement;
        var emailTitle = document.getElementById(ID.EmailTitle) as HTMLTitleElement;
        var emailSubject = document.getElementById(ID.EmailSubject) as HTMLHeadingElement;
        var emailBody = document.getElementById(ID.EmailBody) as HTMLDivElement;
        var errorMessage = document.getElementById(ID.ErrorMessage) as HTMLDivElement;

        if(previewButton){
            previewButton.addEventListener("change", () =>{
                if(previewButton.checked)
                {
                    emailTitle.text = inputSubject.value;
                    emailSubject.textContent = inputSubject.value;
                    emailBody.innerHTML = inputBody.innerHTML;
                    input.style.display = "none";

        			if (errorMessage != null) 
                    {
        			    errorMessage.style.display = "none";
        			}

                    preview.style.display = "block";
                    
                    if(previewSubject)
                    {
                        previewSubject.textContent = inputSubject.value;
                    }
                    
                    if(inputCC)
                    {
                        if(inputCC.checked)
                        {
                            previewCC.hidden = false;
                        }
                        else
                        {
                            previewCC.hidden = true;
                        }
                    }   

                    if(inputImage)
                    {
                        if(inputImage.files?.length == 1)
                        {
                            previewImage.hidden = false;
                            previewImage.innerText = "Image: " + inputImage.files.item(0)?.name;
                        }
                        else
                        {
                            previewImage.hidden = true;
                        }
                    }
                }
                else
                {
                    input.style.display = "block";
                    preview.style.display = "none";
        			if (errorMessage != null) {
        			    errorMessage.style.display = "block";
        			}
                }
            })
        }
    }
}

export default Email;