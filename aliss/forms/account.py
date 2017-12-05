from django import forms
from django.contrib.auth import password_validation
from django.urls import reverse_lazy
from django.utils.functional import lazy
from django.utils.safestring import mark_safe

from aliss.models import ALISSUser


def get_accept_terms_and_conditions_label():
    return mark_safe(
            'I agree to the <a href="{url}">Terms and Conditions'.format(
                url=reverse_lazy('terms_and_conditions')
            )
    )

get_accept_terms_and_conditions_label_lazy = lazy(get_accept_terms_and_conditions_label, str)


class SignupForm(forms.ModelForm):
    error_messages = {
        'password_mismatch': "The two password fields didn't match.",
    }

    password1 = forms.CharField(
        label="Password",
        strip=False,
        widget=forms.PasswordInput,
    )
    password2 = forms.CharField(
        label="Password confirmation",
        widget=forms.PasswordInput,
        strip=False,
    )

    accept_terms_and_conditions = forms.BooleanField(
        required=True,
        label=get_accept_terms_and_conditions_label_lazy()
    )

    class Meta:
        model = ALISSUser
        fields = (
            'email',
            'username',
            'password1',
            'password2',
            'accept_terms_and_conditions'
        )

    def clean_password2(self):
        password1 = self.cleaned_data.get("password1")
        password2 = self.cleaned_data.get("password2")
        if password1 and password2 and password1 != password2:
            raise forms.ValidationError(
                self.error_messages['password_mismatch'],
                code='password_mismatch',
            )
        self.instance.username = self.cleaned_data.get('username')
        password_validation.validate_password(self.cleaned_data.get('password2'), self.instance)
        return password2


class UpdateForm(forms.ModelForm):
    class Meta:
        model = ALISSUser
        fields = ('email', 'username')
