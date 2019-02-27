package com.microsoft.azure.iot.hackathon.accompany.common.models;

public class AcknowledgeRequest {
    public String careTakerUserId;

    public String careRecipientId;

    public String action;

    public AccompanyLocation careTakerLocation;

    public AccompanyLocation careRecipientLocation;
}