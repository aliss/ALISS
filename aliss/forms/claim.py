from django import forms

from aliss.models import Claim


class ClaimUpdateForm(forms.ModelForm):
    status = forms.ChoiceField(
                choices=(
                    (Claim.CONFIRMED, "Confirm Claim"),
                    (Claim.DENIED, "Deny Claim"),
                ),
                widget=forms.RadioSelect()
             )

    class Meta:
        model = Claim
        fields = (
            'status',
        )
