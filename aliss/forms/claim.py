from django import forms

from aliss.models import Claim


class ClaimForm(forms.Form):
    comment = forms.CharField(
                widget=forms.Textarea(),
                label="What is your role in the organisation?")
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

