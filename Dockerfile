FROM postgres
ENV POSTGRES_PASSWORD docker
ENV POSTGRES_DB ChangeCommunication
COPY dump.sql /docker-entrypoint-initdb.d/
