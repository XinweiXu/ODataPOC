--
-- PostgreSQL database dump
--

-- Dumped from database version 13.3 (Debian 13.3-1.pgdg100+1)
-- Dumped by pg_dump version 13.3

-- Started on 2021-06-28 10:51:16

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'ISO_8859_5';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- TOC entry 214 (class 1255 OID 16385)
-- Name: get_message_by_id(bigint); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_message_by_id(_messageid bigint) RETURNS TABLE(message_id bigint, category_id smallint, important boolean, require_acknowledgement boolean, allow_reminder boolean, is_expired boolean, expiry timestamp without time zone, sent timestamp without time zone, sender uuid, status_id smallint, status_name character varying, message_detail json, related_content json, category_name json, recipient_detail json)
    LANGUAGE plpgsql ROWS 1
    AS $$
                BEGIN			
	
	                                RETURN QUERY
	
	                                -- getting subject body detail as a complete json
	                                WITH 	
	                                cte_message_detail_json
	                                AS (
		                                SELECT md.message_id
				                                ,json_agg(
						                                jsonb_build_object('LanguageCode',md.language_code,
											                                  'Subject',md.subject,
											                                  'Body',md.body)
				                                 ) AS message_detail				
		                                FROM message_details md
		                                WHERE md.message_id=_messageId AND md.is_enabled=true					
		                                GROUP BY md.message_id
	                                ),	 	
	                                cte_message_content_json
	                                AS (
		                                SELECT mc.message_id				
				                                ,json_agg(
						                                jsonb_build_object('ContentID',mc.content_id,
								  			                                'ContentTypeID',mc.content_type_id)
				                                 ) AS related_content
		                                FROM message_contents mc 
		                                WHERE mc.message_id=_messageId AND mc.is_enabled=true	
		                                GROUP BY mc.message_id
	                                ),
	                                cte_message_recipitents
	                                AS (		                
						                SELECT mr.message_id,json_agg
								                (
									                jsonb_build_object('UserId',mr.user_id, 'GroupId',mr.group_id)
								                ) recipient_detail	
						                FROM message_recipients mr
						                WHERE mr.message_id=_messageId AND mr.is_enabled=true	
						                GROUP BY mr.message_id
	                                )
	                                -- combine all result set return to caller
	
	                                SELECT mes.id AS message_id
			                                ,mes.category_id
			                                ,mes.is_important
			                                ,mes.requires_acknowledgement
			                                ,mes.allow_reminder
			                                ,mes.is_expired			
			                                ,mes.expiry
			                                ,mes.sent
			                                ,mes.sender_id AS sender
			                                ,mes.status_id
			                                ,ms.status_name					
			                                ,md.message_detail
			                                ,mc.related_content
			                                ,cat.name AS category_name		
			                                ,mr.recipient_detail
			                                 -- recipient_detail json is incorrect - need to change
	                                FROM messages mes	
	                                INNER JOIN message_status ms on ms.id=mes.status_id	
	                                INNER JOIN categories cat ON cat.id=mes.category_id 		
	                                LEFT JOIN cte_message_recipitents mr ON mr.message_id=mes.id
	                                LEFT JOIN cte_message_detail_json md ON md.message_id=mes.id
	                                LEFT JOIN cte_message_content_json mc ON mc.message_id=mes.id
	                                WHERE mes.id=_messageId 
			                                AND mes.is_enabled=true 
			                                AND ms.is_enabled =true 
			                                AND cat.is_enabled=true;
	
                                END
                $$;


ALTER FUNCTION public.get_message_by_id(_messageid bigint) OWNER TO postgres;

--
-- TOC entry 227 (class 1255 OID 24707)
-- Name: get_messages(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_messages() RETURNS TABLE(message_id bigint, category_id smallint, important boolean, require_acknowledgement boolean, is_expired boolean, expiry timestamp without time zone, sent timestamp without time zone, sender uuid, status_id smallint, status_name character varying, message_detail json, related_content json, category_name json, recipient_detail json, engagement_totals json)
    LANGUAGE plpgsql
    AS $$
BEGIN		
		
	--just for temp purpose remove later (when row will start insert into message recipient table)
	REFRESH MATERIALIZED VIEW message_list_engagement;

	RETURN QUERY

	-- getting subject body detail as a complete json
	WITH 	
	cte_message_detail_json
	AS (
		SELECT md.message_id
			  ,json_agg(json_build_object('Language',md.language_code,
								   'Subject',md.subject,
								   'Body',md.body))  AS message_detail
		FROM message_details md 
		WHERE md.is_enabled=true 
		GROUP BY md.message_id
	),	 	
	cte_message_content_json
	AS (
		SELECT mc.message_id				
				,json_agg(
						jsonb_build_object('ContentID',mc.content_id,
								  			'ContentTypeID',mc.content_type_id)
				 ) AS related_content
		FROM message_contents mc 
		WHERE mc.is_enabled=true	
		GROUP BY mc.message_id
	),
	cte_message_recipitents
	AS (
		SELECT  rec.message_id,json_agg(rec.recipient_detail) AS recipient_detail
		FROM (
			SELECT mr.message_id
				, jsonb_build_object('RecipientID',UNNEST(mr.user_id), 'RecipientTypeID',1) recipient_detail	
			FROM message_recipients mr
			WHERE mr.is_enabled=true	
			UNION
			SELECT mr.message_id
				, jsonb_build_object('RecipientID',UNNEST(mr.group_id), 'RecipientTypeID',2)		
			FROM message_recipients mr
			WHERE mr.is_enabled=true	
		) rec
		GROUP BY rec.message_id
	)
	-- combine all result set return to caller	

	SELECT mes.id as message_id
				,mes.category_id
				,mes.is_important
				,mes.requires_acknowledgement
				,mes.is_expired
				,mes.expiry
				,mes.sent
				,mes.sender_id sender
				,mes.status_id
				,ms.status_name										
				,md.message_detail
				,mc.related_content
				,cat.name AS category_name		
				,mr.recipient_detail								
				,json_build_object('Recipients', cv.total_recipient,
							'Read', cv.total_read,
							'Acknowledged', cv.total_ack)  as engagement_totals 
	FROM messages mes	
	INNER JOIN message_status ms on ms.id=mes.status_id	
	INNER JOIN categories cat ON cat.id=mes.category_id 				
	INNER JOIN message_list_engagement cv ON cv.message_id=mes.id					
	LEFT JOIN cte_message_recipitents mr ON mr.message_id=mes.id
	LEFT JOIN cte_message_detail_json md ON md.message_id=mes.id
	LEFT JOIN cte_message_content_json mc ON mc.message_id=mes.id
	WHERE mes.is_enabled=true and ms.is_enabled =true and cat.is_enabled=true
	ORDER BY COALESCE(mes.sent,mes.created) DESC;
END
$$;


ALTER FUNCTION public.get_messages() OWNER TO postgres;

--
-- TOC entry 215 (class 1255 OID 24706)
-- Name: get_messages1(integer, text); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.get_messages1(statusid integer, languagecode text) RETURNS TABLE(message_id bigint, category_id smallint, important boolean, require_acknowledgement boolean, is_expired boolean, expiry timestamp without time zone, sent timestamp without time zone, sender uuid, status_id smallint, status_name character varying, message_detail json, related_content json, category_name json, recipient_detail json, engagement_totals json)
    LANGUAGE plpgsql
    AS $$
BEGIN		
		
	--just for temp purpose remove later (when row will start insert into message recipient table)
	REFRESH MATERIALIZED VIEW message_list_engagement;

	RETURN QUERY

	-- getting subject body detail as a complete json
	WITH 	
	cte_message_detail_json
	AS (
		SELECT md.message_id
			  ,json_agg(json_build_object('Language',md.language_code,
								   'Subject',md.subject,
								   'Body',md.body))  AS message_detail
		FROM message_details md 
		WHERE md.is_enabled=true 
		GROUP BY md.message_id
	),	 	
	cte_message_content_json
	AS (
		SELECT mc.message_id				
				,json_agg(
						jsonb_build_object('ContentID',mc.content_id,
								  			'ContentTypeID',mc.content_type_id)
				 ) AS related_content
		FROM message_contents mc 
		WHERE mc.is_enabled=true	
		GROUP BY mc.message_id
	),
	cte_message_recipitents
	AS (
		SELECT  rec.message_id,json_agg(rec.recipient_detail) AS recipient_detail
		FROM (
			SELECT mr.message_id
				, jsonb_build_object('RecipientID',UNNEST(mr.user_id), 'RecipientTypeID',1) recipient_detail	
			FROM message_recipients mr
			WHERE mr.is_enabled=true	
			UNION
			SELECT mr.message_id
				, jsonb_build_object('RecipientID',UNNEST(mr.group_id), 'RecipientTypeID',2)		
			FROM message_recipients mr
			WHERE mr.is_enabled=true	
		) rec
		GROUP BY rec.message_id
	)
	-- combine all result set return to caller	

	SELECT mes.id as message_id
				,mes.category_id
				,mes.is_important
				,mes.requires_acknowledgement
				,mes.is_expired
				,mes.expiry
				,mes.sent
				,mes.sender_id sender
				,mes.status_id
				,ms.status_name										
				,md.message_detail
				,mc.related_content
				,cat.name AS category_name		
				,mr.recipient_detail								
				,json_build_object('Recipients', cv.total_recipient,
							'Read', cv.total_read,
							'Acknowledged', cv.total_ack)  as engagement_totals 
	FROM messages mes	
	INNER JOIN message_status ms on ms.id=mes.status_id	
	INNER JOIN categories cat ON cat.id=mes.category_id 				
	INNER JOIN message_list_engagement cv ON cv.message_id=mes.id	
    INNER JOIN message_details d ON d.message_id=mes.id		
	LEFT JOIN cte_message_recipitents mr ON mr.message_id=mes.id
	LEFT JOIN cte_message_detail_json md ON md.message_id=mes.id
	LEFT JOIN cte_message_content_json mc ON mc.message_id=mes.id
	WHERE mes.is_enabled=true and ms.is_enabled =true and cat.is_enabled=true
	and mes.status_id=statusId
	and d.language_code=languageCode
	ORDER BY COALESCE(mes.sent,mes.created) DESC;
END
$$;


ALTER FUNCTION public.get_messages1(statusid integer, languagecode text) OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 201 (class 1259 OID 24589)
-- Name: categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categories (
    id smallint NOT NULL,
    name json,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.categories OWNER TO postgres;

--
-- TOC entry 200 (class 1259 OID 24587)
-- Name: categories_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.categories ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.categories_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 209 (class 1259 OID 24651)
-- Name: message_contents; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.message_contents (
    id bigint NOT NULL,
    content_id uuid NOT NULL,
    content_type_id uuid NOT NULL,
    message_id bigint NOT NULL,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.message_contents OWNER TO postgres;

--
-- TOC entry 208 (class 1259 OID 24649)
-- Name: message_contents_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.message_contents ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.message_contents_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 207 (class 1259 OID 24633)
-- Name: message_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.message_details (
    id bigint NOT NULL,
    language_code character varying(10),
    subject character varying(200),
    body text,
    message_id bigint NOT NULL,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.message_details OWNER TO postgres;

--
-- TOC entry 206 (class 1259 OID 24631)
-- Name: message_details_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.message_details ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.message_details_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 213 (class 1259 OID 24684)
-- Name: message_recipient_details; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.message_recipient_details (
    id bigint NOT NULL,
    recipient_id bigint NOT NULL,
    placeholder_value json,
    is_expired boolean,
    received timestamp without time zone,
    expiry timestamp without time zone,
    read timestamp without time zone,
    acknowledged timestamp without time zone,
    message_detail_id bigint NOT NULL,
    message_recipient_id bigint NOT NULL,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.message_recipient_details OWNER TO postgres;

--
-- TOC entry 212 (class 1259 OID 24682)
-- Name: message_recipient_details_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.message_recipient_details ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.message_recipient_details_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 211 (class 1259 OID 24666)
-- Name: message_recipients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.message_recipients (
    id bigint NOT NULL,
    user_id uuid[],
    group_id uuid[],
    name character varying(200),
    message_id bigint NOT NULL,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.message_recipients OWNER TO postgres;

--
-- TOC entry 210 (class 1259 OID 24664)
-- Name: message_recipients_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.message_recipients ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.message_recipients_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 203 (class 1259 OID 24601)
-- Name: message_status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.message_status (
    id smallint NOT NULL,
    status_name character varying(50) NOT NULL,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.message_status OWNER TO postgres;

--
-- TOC entry 202 (class 1259 OID 24599)
-- Name: message_status_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.message_status ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.message_status_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 205 (class 1259 OID 24610)
-- Name: messages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.messages (
    id bigint NOT NULL,
    allow_reminder boolean,
    tenant_id uuid,
    sender_id uuid NOT NULL,
    category_id smallint NOT NULL,
    is_completed boolean DEFAULT false NOT NULL,
    is_important boolean NOT NULL,
    is_expired boolean DEFAULT false NOT NULL,
    requires_acknowledgement boolean NOT NULL,
    status_id smallint NOT NULL,
    expiry timestamp without time zone,
    read timestamp without time zone,
    acknowledged timestamp without time zone,
    sent timestamp without time zone,
    is_enabled boolean DEFAULT true NOT NULL,
    created_by uuid NOT NULL,
    modified_by uuid,
    created timestamp without time zone DEFAULT timezone('utc'::text, now()) NOT NULL,
    modified timestamp without time zone
);


ALTER TABLE public.messages OWNER TO postgres;

--
-- TOC entry 204 (class 1259 OID 24608)
-- Name: messages_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

ALTER TABLE public.messages ALTER COLUMN id ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public.messages_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- TOC entry 3021 (class 0 OID 24589)
-- Dependencies: 201
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.categories (id, name, is_enabled, created_by, modified_by, created, modified) FROM stdin;
1	[{"Name":"Downtime","Language":"en-us"}]	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
2	[{"Name":"Onboarding","Language":"en-us"}]	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
3	[{"Name":"Rollout","Language":"en-us"}]	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
\.


--
-- TOC entry 3029 (class 0 OID 24651)
-- Dependencies: 209
-- Data for Name: message_contents; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.message_contents (id, content_id, content_type_id, message_id, is_enabled, created_by, modified_by, created, modified) FROM stdin;
\.


--
-- TOC entry 3027 (class 0 OID 24633)
-- Dependencies: 207
-- Data for Name: message_details; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.message_details (id, language_code, subject, body, message_id, is_enabled, created_by, modified_by, created, modified) FROM stdin;
1	en-Us	subject english	body english	1	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:36.139493	\N
2	en-Us	subject english	body english	2	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:37.660857	\N
3	en-Us	subject english	body english	3	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:38.70693	\N
5	en-Us	subject english	body english	5	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:42.260322	\N
6	en-Gb	subject english	body english	1	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:36.139493	\N
4	en-Gb	subject english	body english	4	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:39.68679	\N
\.


--
-- TOC entry 3033 (class 0 OID 24684)
-- Dependencies: 213
-- Data for Name: message_recipient_details; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.message_recipient_details (id, recipient_id, placeholder_value, is_expired, received, expiry, read, acknowledged, message_detail_id, message_recipient_id, is_enabled, created_by, modified_by, created, modified) FROM stdin;
\.


--
-- TOC entry 3031 (class 0 OID 24666)
-- Dependencies: 211
-- Data for Name: message_recipients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.message_recipients (id, user_id, group_id, name, message_id, is_enabled, created_by, modified_by, created, modified) FROM stdin;
\.


--
-- TOC entry 3023 (class 0 OID 24601)
-- Dependencies: 203
-- Data for Name: message_status; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.message_status (id, status_name, is_enabled, created_by, modified_by, created, modified) FROM stdin;
1	Expired	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
2	Draft	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
3	Active	t	00000000-0000-0000-0000-000000000000	\N	2021-06-24 12:27:56.323893	\N
\.


--
-- TOC entry 3025 (class 0 OID 24610)
-- Dependencies: 205
-- Data for Name: messages; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.messages (id, allow_reminder, tenant_id, sender_id, category_id, is_completed, is_important, is_expired, requires_acknowledgement, status_id, expiry, read, acknowledged, sent, is_enabled, created_by, modified_by, created, modified) FROM stdin;
1	t	\N	3fa85f64-5717-4562-b3fc-2c963f66afa6	1	f	t	t	t	1	2021-06-07 02:51:48.009	\N	\N	\N	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:36.139493	\N
2	t	\N	3fa85f64-5717-4562-b3fc-2c963f66afa6	1	f	t	t	t	1	2021-06-07 02:51:48.009	\N	\N	\N	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:37.660857	\N
3	t	\N	3fa85f64-5717-4562-b3fc-2c963f66afa6	1	f	t	t	t	1	2021-06-07 02:51:48.009	\N	\N	\N	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:38.70693	\N
5	t	\N	3fa85f64-5717-4562-b3fc-2c963f66afa6	1	f	t	t	t	2	2021-06-07 02:51:48.009	\N	\N	\N	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:42.260322	\N
4	t	\N	3fa85f64-5717-4562-b3fc-2c963f66afa6	1	f	t	t	t	3	2021-06-07 02:51:48.009	\N	\N	\N	t	3fa85f64-5717-4562-b3fc-2c963f66afa6	\N	2021-06-21 16:28:39.68679	\N
\.


--
-- TOC entry 3039 (class 0 OID 0)
-- Dependencies: 200
-- Name: categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.categories_id_seq', 1, false);


--
-- TOC entry 3040 (class 0 OID 0)
-- Dependencies: 208
-- Name: message_contents_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.message_contents_id_seq', 1, false);


--
-- TOC entry 3041 (class 0 OID 0)
-- Dependencies: 206
-- Name: message_details_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.message_details_id_seq', 1, false);


--
-- TOC entry 3042 (class 0 OID 0)
-- Dependencies: 212
-- Name: message_recipient_details_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.message_recipient_details_id_seq', 1, false);


--
-- TOC entry 3043 (class 0 OID 0)
-- Dependencies: 210
-- Name: message_recipients_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.message_recipients_id_seq', 1, false);


--
-- TOC entry 3044 (class 0 OID 0)
-- Dependencies: 202
-- Name: message_status_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.message_status_id_seq', 1, false);


--
-- TOC entry 3045 (class 0 OID 0)
-- Dependencies: 204
-- Name: messages_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.messages_id_seq', 1, false);


--
-- TOC entry 2863 (class 2606 OID 24598)
-- Name: categories pk_categories; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT pk_categories PRIMARY KEY (id);


--
-- TOC entry 2875 (class 2606 OID 24657)
-- Name: message_contents pk_message_contents; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_contents
    ADD CONSTRAINT pk_message_contents PRIMARY KEY (id);


--
-- TOC entry 2872 (class 2606 OID 24642)
-- Name: message_details pk_message_details; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_details
    ADD CONSTRAINT pk_message_details PRIMARY KEY (id);


--
-- TOC entry 2882 (class 2606 OID 24693)
-- Name: message_recipient_details pk_message_recipient_details; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_recipient_details
    ADD CONSTRAINT pk_message_recipient_details PRIMARY KEY (id);


--
-- TOC entry 2878 (class 2606 OID 24675)
-- Name: message_recipients pk_message_recipients; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_recipients
    ADD CONSTRAINT pk_message_recipients PRIMARY KEY (id);


--
-- TOC entry 2865 (class 2606 OID 24607)
-- Name: message_status pk_message_status; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_status
    ADD CONSTRAINT pk_message_status PRIMARY KEY (id);


--
-- TOC entry 2869 (class 2606 OID 24618)
-- Name: messages pk_messages; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT pk_messages PRIMARY KEY (id);


--
-- TOC entry 2873 (class 1259 OID 24663)
-- Name: ix_message_contents_message_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_message_contents_message_id ON public.message_contents USING btree (message_id);


--
-- TOC entry 2870 (class 1259 OID 24648)
-- Name: ix_message_details_message_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_message_details_message_id ON public.message_details USING btree (message_id);


--
-- TOC entry 2879 (class 1259 OID 24704)
-- Name: ix_message_recipient_details_message_detail_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_message_recipient_details_message_detail_id ON public.message_recipient_details USING btree (message_detail_id);


--
-- TOC entry 2880 (class 1259 OID 24705)
-- Name: ix_message_recipient_details_message_recipient_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_message_recipient_details_message_recipient_id ON public.message_recipient_details USING btree (message_recipient_id);


--
-- TOC entry 2876 (class 1259 OID 24681)
-- Name: ix_message_recipients_message_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_message_recipients_message_id ON public.message_recipients USING btree (message_id);


--
-- TOC entry 2866 (class 1259 OID 24629)
-- Name: ix_messages_category_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_messages_category_id ON public.messages USING btree (category_id);


--
-- TOC entry 2867 (class 1259 OID 24630)
-- Name: ix_messages_status_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX ix_messages_status_id ON public.messages USING btree (status_id);


--
-- TOC entry 2886 (class 2606 OID 24658)
-- Name: message_contents fk_msgcontents_msgs_msgid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_contents
    ADD CONSTRAINT fk_msgcontents_msgs_msgid FOREIGN KEY (message_id) REFERENCES public.messages(id) ON DELETE CASCADE;


--
-- TOC entry 2885 (class 2606 OID 24643)
-- Name: message_details fk_msgdetails_msgs_msgid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_details
    ADD CONSTRAINT fk_msgdetails_msgs_msgid FOREIGN KEY (message_id) REFERENCES public.messages(id) ON DELETE CASCADE;


--
-- TOC entry 2888 (class 2606 OID 24694)
-- Name: message_recipient_details fk_msgrcpntdetails_msgdetails_msgdetailid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_recipient_details
    ADD CONSTRAINT fk_msgrcpntdetails_msgdetails_msgdetailid FOREIGN KEY (message_detail_id) REFERENCES public.message_details(id) ON DELETE CASCADE;


--
-- TOC entry 2889 (class 2606 OID 24699)
-- Name: message_recipient_details fk_msgrcpntdetails_msgrcpnts_msgrcpntid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_recipient_details
    ADD CONSTRAINT fk_msgrcpntdetails_msgrcpnts_msgrcpntid FOREIGN KEY (message_recipient_id) REFERENCES public.message_recipients(id) ON DELETE CASCADE;


--
-- TOC entry 2887 (class 2606 OID 24676)
-- Name: message_recipients fk_msgrcpnts_msgs_msgid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.message_recipients
    ADD CONSTRAINT fk_msgrcpnts_msgs_msgid FOREIGN KEY (message_id) REFERENCES public.messages(id) ON DELETE CASCADE;


--
-- TOC entry 2883 (class 2606 OID 24619)
-- Name: messages fk_msgs_cats_catid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT fk_msgs_cats_catid FOREIGN KEY (category_id) REFERENCES public.categories(id) ON DELETE CASCADE;


--
-- TOC entry 2884 (class 2606 OID 24624)
-- Name: messages fk_msgs_msgstat_statid; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.messages
    ADD CONSTRAINT fk_msgs_msgstat_statid FOREIGN KEY (status_id) REFERENCES public.message_status(id) ON DELETE CASCADE;


-- Completed on 2021-06-28 10:51:17

--
-- PostgreSQL database dump complete
--

