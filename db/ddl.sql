-- DDL script to create the tenantsvc database, its schemas, and tables for managing 
-- tenant information and user accounts.
-- 
-- This script sets up the foundational database structure for the tenant service from scratch.
-- ==============================================================================================
-- Version: 1.0.613 

-- Creates the tenantsvc database with specific settings such as owner, encoding, 
-- collation, and locale provider.
CREATE DATABASE tenantsvc
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.utf8'
    LC_CTYPE = 'en_US.utf8'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;


-- Creates the infrastructure schema for the tenant service, which contains tables 
-- and other database objects related to the service's internal operations and management.
CREATE SCHEMA infra
    AUTHORIZATION postgres;

CREATE SCHEMA application
    AUTHORIZATION postgres;


-- Table: infra.user - administrator's login accounts to manipulate tenants

CREATE TABLE IF NOT EXISTS infra.user
(
    id uuid NOT NULL,
    username character varying(32) COLLATE pg_catalog."default" NOT NULL,
    fullName character varying(256) COLLATE pg_catalog."default" NOT NULL,
    email character varying(256) COLLATE pg_catalog."default" NOT NULL,
    passwordHash character varying(128) COLLATE pg_catalog."default",
    admin boolean NOT NULL DEFAULT false,
    status smallint NOT NULL DEFAULT 0,
    CONSTRAINT user_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS infra.user
    OWNER to postgres;

CREATE TABLE IF NOT EXISTS infra.acl
(
    user_id uuid NOT NULL,
    tenant_id character varying(10) COLLATE pg_catalog."default" NOT NULL,
    write boolean NOT NULL,
    CONSTRAINT acl_pkey PRIMARY KEY (user_id, tenant_id)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS infra.acl
    OWNER to postgres;    


-- Table: application.tenant - tenants' information

CREATE TABLE IF NOT EXISTS application.tenant
(
    id character varying(10) COLLATE pg_catalog."default" NOT NULL,
    name character varying(256) COLLATE pg_catalog."default" NOT NULL,
    tax_code character varying(16) COLLATE pg_catalog."default" NOT NULL,
    subscription_date date NOT NULL DEFAULT CURRENT_DATE,
    subscriber_email character varying(64) COLLATE pg_catalog."default" NOT NULL,
    license_id uuid NOT NULL,
    disposal_date date,
    status smallint NOT NULL,
    CONSTRAINT tenant_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default;    

CREATE TABLE IF NOT EXISTS application.license
(
    id uuid NOT NULL,
    serialNumber character varying(32) COLLATE pg_catalog."default" NOT NULL,
    name character varying(64) COLLATE pg_catalog."default" NOT NULL,
    max_users integer NOT NULL,
    CONSTRAINT tenant_license_pkey PRIMARY KEY (id)
)

TABLESPACE pg_default; 

ALTER TABLE IF EXISTS infra.user
    ADD CONSTRAINT user_ukey_username UNIQUE (username);


-- foreign keys
ALTER TABLE IF EXISTS infra.acl
    ADD CONSTRAINT acl_fkey_tenant FOREIGN KEY (tenant_id)
    REFERENCES application.tenant (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS infra.acl
    ADD CONSTRAINT acl_fkey_user FOREIGN KEY (user_id)
    REFERENCES infra.user (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS application.tenant
    ADD CONSTRAINT tenant_fkey_license FOREIGN KEY (license_id)
    REFERENCES application.license (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION 
    NOT VALID;    