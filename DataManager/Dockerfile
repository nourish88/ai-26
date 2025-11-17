ARG APP_HOME=/app
ARG APP_USER=app

FROM python:3.12.11-slim-bookworm AS build
ARG APP_HOME
ARG APP_USER

ENV PIP_DEFAULT_TIMEOUT=100 \
    PIP_DISABLE_PIP_VERSION_CHECK=1 \
    PIP_NO_CACHE_DIR=1

WORKDIR $APP_HOME
COPY DataManager/requirements.txt .

COPY certs/aa.crt /usr/local/share/ca-certificates/aa.crt
COPY certs/pypi.org.crt /usr/local/share/ca-certificates/pypi.org.crt
RUN set -ex \
    && addgroup --system --gid 1001 $APP_USER \
    && adduser --system --uid 1001 --gid 1001 --no-create-home $APP_USER \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && echo 'APT::Get::AllowUnauthenticated "true";' > /etc/apt/apt.conf.d/99allow-unauthenticated \
    && apt-get update --allow-releaseinfo-change 2>&1 | grep -v "GPG error" | grep -v "not signed" || true \
    && apt-get install -y --no-install-recommends ca-certificates gnupg2 \
    && rm -f /etc/apt/apt.conf.d/99allow-unauthenticated \
    && apt-get update 2>&1 | grep -v "GPG error" | grep -v "not signed" || true \
    && apt-get upgrade -y \
    && update-ca-certificates \
    && pip install -r requirements.txt \
    && apt-get autoremove -y \
    && apt-get clean -y \
    && rm -rf /var/lib/apt/lists/*

FROM build AS main
ARG APP_HOME
ARG APP_USER

ENV PYTHONUNBUFFERED=1

WORKDIR $APP_HOME

COPY DataManager/server.py .
COPY DataManager/src src
RUN chown -R $APP_USER:$APP_USER $APP_HOME

CMD ["python", "server.py"]
USER $APP_USER
