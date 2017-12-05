from django.conf import settings
from django.http import HttpResponse

from twilio.twiml.messaging_response import MessagingResponse

from rest_framework import generics
from rest_framework.exceptions import ParseError
from rest_framework.renderers import JSONRenderer
from rest_framework.views import APIView

from elasticsearch_dsl import Search
from elasticsearch_dsl.connections import connections

from .paginator import ESPageNumberPagination
from .serializers import (
    SearchSerializer,
    SearchInputSerializer,
    CategorySerializer,
    ServiceAreaSerializer
)

from aliss.models import Category, ServiceArea
from aliss.search import (
    filter_by_query,
    filter_by_postcode,
    filter_by_categories,
    filter_by_service_areas
)


class SearchView(generics.ListAPIView):
    serializer_class = SearchSerializer
    pagination_class = ESPageNumberPagination

    def list(self, request, *args, **kwargs):
        input_serializer = SearchInputSerializer(data=request.query_params)
        input_serializer.is_valid(raise_exception=True)

        self.input_data = input_serializer.validated_data

        return super(SearchView, self).list(request, *args, **kwargs)

    def filter_queryset(self, queryset):
        radius = self.input_data.get('radius')
        postcode = self.input_data.get('postcode')

        queryset = filter_by_postcode(queryset, postcode, radius)

        query = self.input_data.get('query', None)
        categories = self.input_data.get('categories', None)
        service_areas = self.input_data.get('service_areas', None)

        if query:
            queryset = filter_by_query(queryset, query)
        if categories:
            queryset = filter_by_categories(queryset, categories)
        if service_areas:
            queryset = filter_by_service_areas(queryset, service_areas)

        return queryset

    def get_queryset(self, *args, **kwargs):
        connections.create_connection(
            hosts=[settings.ELASTICSEARCH_URL], timeout=20, http_auth=(settings.ELASTICSEARCH_USERNAME, settings.ELASTICSEARCH_PASSWORD))
        queryset = Search(index='search', doc_type='service')
        return queryset


class CategoryListView(generics.ListAPIView):
    queryset = Category.objects.all()
    serializer_class = CategorySerializer
    pagination_class = None


class ServiceAreaListView(generics.ListAPIView):
    queryset = ServiceArea.objects.all()
    serializer_class = ServiceAreaSerializer
    pagination_class = None


class TextView(APIView):
    def get(self, request, format=None):
        from_number = request.query_params.get('From', None)
        body = request.query_params.get('Body', None)

        resp = MessagingResponse()

        if body in ['1', '2', '3', '4', '5', '6', '7']:
            request.session['category'] = body
            resp.message("What is your postcode?")
        elif request.session.get('category', None):
            request.session['postcode'] = body

            epic = "Epic 360\n"\
                   "Epic 360 will offer advice to help you manage money.\n"\
                   "Phone: 0141 630 4325\n"\
                   "Website: http://www.epic360.co.uk"

            gain = "GAIN (Glasgow Advice and Information Network)\n"\
                   "Advice on a wide range of topics such as debt, housing, managing your money and changes to the benefits system.\n"\
                   "Phone: 0808 801 1011\n"\
                   "Website: http://www.gain4u.org.uk"

            CAB = "Glasgow Central Citizens' Advice Centre\n"\
                  "Free, confidential, impartial and independent information and advice, will work with you to help resolve issues.\n"\
                  "Phone: 0141 552 5556\n"\
                  "Website: http://www.glasgowcentralcab.org.uk"

            StepChange = "Step Change Debt Charity\n"\
                         "Offer expert, tailored advice and practical solutions to problem debt.\n"\
                         "Phone: 0800 138 1111\n"\
                         "Website: https://www.stepchange.org"

            WeeGlasgowLoans = "Wee Glasgow Loans\n"\
                              "Small low intrest short term loans to help spread the cost of borrowing.\n"\
                              "Phone: 0141 881 8731\n"\
                              "Website: http://www.weeglasgowloan.scot"

            CAP = "Christians Against Poverty\n"\
                  "A charity that gives free help to anyone in debt.\n"\
                  "Phone: 0800 328 0006\n"\
                  "Website: https://capuk.org"

            ScottishWelfareFund = "Scottish Welfare Fund\n"\
                                  "Provides grants that do not have to be repaid.\n"\
                                  "Phone:  0141 276 1177\n"\
                                  "Website: https://www.glasgow.gov.uk/index.aspx?articleid=17160"

            GHN = "Glasgow Homelessness Network\n"\
                  "We want everyone to have a safe and secure home from which to build and live their lives.\n"\
                  "Phone: 0141 420 7272\n"\
                  "Website: http://www.ghn.org.uk"

            BigIssue = "The Big issue Foundation\n"\
                       "Supporting Big Issue vendors in the self-help process of buying & selling the Big Issue Magazine.\n"\
                       "Phone: 0141 553 0924\n"\
                       "Website: https://www.bigissue.com"

            GWA = "Glasgow Womens Aid\n"\
                  "Provide support to women, children and young people who  experience domestic abuse.\n"\
                  "Phone: 0141 533 2022\n"\
                  "Website: http://www.glasgowwomensaid.org.uk"

            Aims = "AMIS (abused men in Scotland)\n"\
                   "Provide direct support to men experiencing domestic abuse.\n"\
                   "Phone: 0808 800 0024\n"\
                   "Website: http://www.abusedmeninscotland.org"

            LGBTHelpline = "National LGBT Domestic Abuse Helpline\n"\
                           "Emotional and practical support for LGBT people experiencing domestic abuse.\n"\
                           "Phone: 0800 999 5428\n"\
                           "Website: http://www.galop.org.uk/domesticabuse/"

            SRC = "Scottish Refugee Council\n"\
                  "Offer direct advice services to people seeking asylum and refugees.\n"\
                  "Phone: 0141 248 9799\n"\
                  "Website: http://www.scottishrefugeecouncil.org.uk"

            PositiveAction = "Positive Action\n"\
                             "Assist those seeking refuge to overcome crisis situations.\n"\
                             "Phone:0141 353 2220\n"\
                             "Website: http://www.paih.org"

            MIN = "Maryhill Integration Network\n"\
                  "The Network brings communities together through, art, social, cultural and educational groups and projects, offering people a chance to learn new skills, meet new people, share experiences and take part in worthwhile activities to improve their communities.\n"\
                  "Phone: 0141 946 9106\n"\
                  "Website: http://www.maryhillintegration.org.uk/"

            NHS = "NHS 24\n"\
                  "NHS 24 provides comprehensive health information and self care advice to the people of Scotland.\n"\
                  "Phone: 111\n"\
                  "Website: http://www.nhs24.com"

            GAMH = "Glasgow Association for Mental Health\n"\
                   "We promote the mental health and wellbeing of people and their communities; providing more than 2000 hours of community based support every week to people in Glasgow.\n"\
                   "Phone: 0141 552 5592\n"\
                   "Website: http://gamh.org.uk"


            if request.session.get('category') == '1':
                resp.message("{0}\n\n{1}\n\n{2}".format(epic, gain, CAB))
            elif request.session.get('category') == '2':
                resp.message("{0}\n\n{1}\n\n{2}".format(CAB, gain, ScottishWelfareFund))
            elif request.session.get('category') == '3':
                resp.message("{0}\n\n{1}\n\n{2}".format(StepChange, CAP, WeeGlasgowLoans))
            elif request.session.get('category') == '4':
                resp.message("{0}\n\n{1}".format(GHN, BigIssue))
            elif request.session.get('category') == '5':
                resp.message("{0}\n\n{1}\n\n{2}".format(CAB, NHS, GAMH))
            elif request.session.get('category') == '6':
                resp.message("{0}\n\n{1}\n\n{2}".format(GWA, Aims, LGBTHelpline))
            elif request.session.get('category') == '7':
                resp.message("{0}\n\n{1}\n\n{2}".format(SRC, PositiveAction, MIN))

            request.session['category'] = None
            request.session['postcode'] = None
        else:
            resp.message("Please choose the type of support you need:\n"\
                         "1. Low Income\n"\
                         "2. Benefit Problems\n"\
                         "3. Debt\n"\
                         "4. Homelessness\n"\
                         "5. Sickness/Ill Health\n"\
                         "6. Domestic Abuse\n"\
                         "7. Refugee/Asylum seeker\n")

        return HttpResponse(str(resp), content_type='text/xml')
