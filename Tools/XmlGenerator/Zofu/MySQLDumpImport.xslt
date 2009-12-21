<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:strip-space elements="*"/>
  <xsl:output method="xml" indent="yes"/>
  
  <xsl:template match="database">
    <xsl:element name="list">
      <xsl:for-each select="*/row">
        <xsl:call-template name="row"/>
      </xsl:for-each>
    </xsl:element>
  </xsl:template>

  <!-- Rows are transformed into something else-->
  <xsl:template name="row">
    <xsl:element name="item">
      <xsl:for-each select="field">
        <xsl:call-template name="field">
          <xsl:with-param name="fieldName" select="@name"/>
        </xsl:call-template>
      </xsl:for-each>
    </xsl:element>
  </xsl:template>

  <!-- Template applied to rowsets-->
  <xsl:template name="field">
    <xsl:param name="fieldName">row</xsl:param>
    <xsl:element name="{$fieldName}">
      <xsl:if test="@xsi:nil = 'true'">
        <xsl:attribute name="xsi:nil">true</xsl:attribute>
      </xsl:if>
      <xsl:value-of select="."/>
    </xsl:element>
  </xsl:template>
</xsl:stylesheet>
