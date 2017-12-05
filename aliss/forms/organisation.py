from django import forms


class OrganisationClaimForm(forms.Form):
    email = forms.EmailField(
                help_text="The email address we should use to verify you as the owner of this organisation")
    phone = forms.CharField(
                help_text="The phone number we should use to verify you as the owner of this organisation")
    comment = forms.CharField(
                widget=forms.Textarea(),
                label="Tell us why you should be allowed to claim this organisation")
