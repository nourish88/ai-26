ARG APP_HOME=/app
ARG APP_USER=app

FROM python:3.12.11-slim-bookworm AS build
ARG APP_HOME
ARG APP_USER

ENV PIP_DEFAULT_TIMEOUT=100 \
    PIP_DISABLE_PIP_VERSION_CHECK=1 \
    PIP_NO_CACHE_DIR=1

WORKDIR $APP_HOME
COPY AIOrchestrator/requirements.txt .
COPY certs/corporate-root-ca.crt /usr/local/share/ca-certificates/corporate-root-ca.crt
COPY certs/corporate-intermediate-ca.crt /usr/local/share/ca-certificates/corporate-intermediate-ca.crt
COPY certs/corporate-subca.crt /usr/local/share/ca-certificates/corporate-subca.crt

RUN set -ex \
    && addgroup --system --gid 1001 $APP_USER \
    && adduser --system --uid 1001 --gid 1001 --no-create-home $APP_USER \
    && apt-get update \
    && apt-get install -y --no-install-recommends ca-certificates \
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

COPY AIOrchestrator/main.py .
COPY AIOrchestrator/applications applications
COPY AIOrchestrator/chat chat
COPY AIOrchestrator/integrations integrations
COPY AIOrchestrator/log log
COPY AIOrchestrator/request request
COPY AIOrchestrator/server server
COPY AIOrchestrator/static static
RUN chown -R $APP_USER:$APP_USER $APP_HOME

CMD ["python", "main.py"]
USER $APP_USER
