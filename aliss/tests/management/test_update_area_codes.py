from django.test import TestCase
from aliss.models import Postcode
from aliss.management.commands.update_area_codes import Command as UpdateAreaCodesCommand


class UpdateAreaCodesTest(TestCase):

    def setUp(self):
        Postcode.objects.create(**{
            'postcode': 'G2 4AA', 'postcode_district': 'G2',
            'postcode_sector': 'G2 4', 'latitude': 55.86523763,
            'longitude': -4.26974322, 'council_area_2011_code': 'S12000046',
            'health_board_area_2014_code': 'S08000021',
            'integration_authority_2016_code': 'S37BORK',
            'place_name': 'Glasgow', 'slug': 'glasgow'
        })

    def test_command_output(self):
        p = Postcode.objects.get(postcode='G2 4AA')
        smalluser_row = ['G2 4AA', 'G2', 'G2 4', '9/7/2004 00:00:00', '', '258058', '665891', '55.86523763', '-4.26974322', 'N', 'S12000046', 'S14000029', 'S17000017', 'S16000117', 'S13002976', 'S08000021', 'S08000007', '09', 'S37000015', 'S00116587', 'S00025582', '6341AJ01', 'S01010267', 'S01003433', 'S02001933', 'S02000655', '0', '0', '', '', '', '', '3170', 'S30000019', 'UKM8', 'UKM82', 'S19001549', '133019', '259', 'S20001250', '133', 'S35000351', 'S09000006', '000', '41', '41', '', 'S12000046', '', '', 'S11000004', 'S22000065', '1', '1', 'Y', '1', '']
        largeuser_row = ['G2 4AA', 'G2', 'G2 4', '1/8/1973 00:00:00', '1/7/1992 00:00:00', 'G2 4AA', '258058', '665891', '55.86523763', '-4.26974322', 'N', 'S12000046', 'S14000029', 'S17000017', 'S16000117', 'S13002976', 'S08000021', 'S08000007', '09', 'S37000015', 'S00116587', 'S00025582', '6341AJ01', 'S01010267', 'S01003433', 'S02001933', 'S02000655', '3170', 'S30000019', 'UKM8', 'UKM82', 'S19001549', '133019', '259', 'S20001250', '133', 'S35000351', 'S09000006', '000', '41', '41', '', 'S12000046', '', '', 'S11000004', 'S22000065', '1', '1', 'N', '']
        c = UpdateAreaCodesCommand()
        c.handle_smalluser_row(smalluser_row)
        c.handle_largeuser_row(largeuser_row)
        p.refresh_from_db()
        self.assertTrue(p.integration_authority_2016_code == 'S37000015')