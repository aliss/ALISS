
from django.core import management
from django.http import HttpResponse
from reportlab.pdfgen import canvas
import reportlab

class printReport(MultipleObjectMixin, TemplateView):
 management.call_command("generate_location_report")

def some_view(request):
    report_pdf = management.call_command("generate_location_report")
    # Create the HttpResponse object with the appropriate PDF headers.
    response = HttpResponse(content_type='application/pdf')
    response['Content-Disposition'] = 'attachment; filename="somefilename.pdf"'

    # Create the PDF object, using the response object as its "file."
    p = canvas.Canvas(response)

    # Draw things on the PDF. Here's where the PDF generation happens.
    # See the ReportLab documentation for the full list of functionality.
    p.drawString(100, 100, report_pdf)

    # Close the PDF object cleanly, and we're done.
    p.showPage()
    p.save()
    return response