from django import forms
from localflavor.gb.forms import GBPostcodeField
from aliss.models import DigestSelection, Postcode, Category, ALISSUser

class DigestSelectionForm(forms.ModelForm):
    postcode = GBPostcodeField(required=True)

    category = forms.ModelChoiceField(
        queryset=Category.objects.all(),
        to_field_name="slug",
        required=False
        )

    def clean(self):
        cleaned_data = super(DigestSelectionForm, self).clean()

        postal_string = cleaned_data.get("postcode")
        category_slug = cleaned_data.get("category")

        return cleaned_data
