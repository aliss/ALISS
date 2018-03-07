from django import forms

from aliss.models import Claim


class ClaimForm(forms.Form):
    email = forms.EmailField(
                help_text="The email address we should use to verify you as the owner of this organisation")
    phone = forms.CharField(
                help_text="The phone number we should use to verify you as the owner of this organisation")
    comment = forms.CharField(
                widget=forms.Textarea(),
                label="Tell us why you should be allowed to claim this organisation")
    data_quality = forms.BooleanField(label="I understand and acknowledge the importance of data quality and agree to follow the guidance outlined in the ALISS Data Standards")

    class Meta:
        error_css_class = 'has-error'

class ClaimUpdateForm(forms.ModelForm):
    status = forms.ChoiceField(
                choices=(
                    (Claim.CONFIRMED, "Confirm Claim"),
                    (Claim.REVOKED, "Revoke Claim"),
                    (Claim.DENIED, "Deny Claim"),
                    (Claim.UNREVIEWED, "Review Claim"),
                ),
                widget=forms.RadioSelect()
             )

    class Meta:
        model = Claim
        fields = (
            'status',
        )
        error_css_class = 'has-error'

