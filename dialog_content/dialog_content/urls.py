from django.conf.urls import patterns, include, url
from django.conf import settings
import os

from django.contrib import admin
admin.autodiscover()

dbfile_name = settings.DATABASES['default']['NAME']

urlpatterns = patterns('',
    # Examples:
    url(r'^dialog_content.sqlite3$', 'django.views.static.serve', 
        {'path': os.path.basename(dbfile_name), 'document_root': os.path.dirname(dbfile_name)}),
    # url(r'^blog/', include('blog.urls')),

    url(r'^', include(admin.site.urls)),
)
