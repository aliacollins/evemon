<?xml version="1.0" encoding="utf-8" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:template match="character">
    <html>
      <head>
        <title>
          Character Sheet :: <xsl:value-of select="@name"/>
        </title>
      </head>
      <body>
        <table>
          <tr>
            <td>
              <xsl:element name="img">
                <xsl:attribute name="src">
                  http://img.eve.is/serv.asp?s=256&amp;c=<xsl:value-of select="@characterID"/>
                </xsl:attribute>
                <xsl:attribute name="style">width:256px;height:256px;</xsl:attribute>
              </xsl:element>
            </td>
            <td>
              <div class="characterName">
                <xsl:value-of select="@name"/>
              </div>
              <div class="characterBio">
                <span class="gender">
                  <xsl:value-of select="gender/child::text()"/>
                </span>
                <span class="race">
                  <xsl:value-of select="race/child::text()"/>
                </span>
                <span class="bloodline">
                  <xsl:value-of select="bloodLine/child::text()"/>
                </span>
              </div>
              <div class="corpInfo">
                Corporation:
                <span class="corpName">
                  <xsl:value-of select="corporationName/child::text()"/>
                </span>
              </div>
              <div class="EVEFolder">
                EVEFolder
                <span class="EVEFolder">
                  <xsl:value-of select="EVEFolder/child::text()"/>
                </span>
              </div>
              <div class="balanceInfo">
                Balance:
                <span class="balance">
                  <xsl:value-of select="balance/child::text()"/> ISK
                </span>
              </div>
              <table class="attributeTable">
                <tr>
                  <td class="attributeTitle intelligenceTitle">Intelligence:</td>
                  <td class="attributeValue intelligenceValue">
                    <xsl:value-of select="attributes/adjustedIntelligence/child::text()"/>
                  </td>
                </tr>
                <tr>
                  <td class="attributeTitle charismaTitle">Charisma:</td>
                  <td class="attributeValue charismaValue">
                    <xsl:value-of select="attributes/adjustedCharisma/child::text()"/>
                  </td>
                </tr>
                <tr>
                  <td class="attributeTitle perceptionTitle">Perception:</td>
                  <td class="attributeValue perceptionValue">
                    <xsl:value-of select="attributes/adjustedPerception/child::text()"/>
                  </td>
                </tr>
                <tr>
                  <td class="attributeTitle memoryTitle">Memory:</td>
                  <td class="attributeValue memoryValue">
                    <xsl:value-of select="attributes/adjustedMemory/child::text()"/>
                  </td>
                </tr>
                <tr>
                  <td class="attributeTitle willpowerTitle">Willpower:</td>
                  <td class="attributeValue willpowerValue">
                    <xsl:value-of select="attributes/adjustedWillpower/child::text()"/>
                  </td>
                </tr>
              </table>
              <xsl:if test="attributeEnhancers/*">
                <table class="implantTable">
                  <xsl:apply-templates select="attributeEnhancers/*"/>
                </table>
              </xsl:if>
            </td>
          </tr>
        </table>
        <table class="skillsTable">
          <xsl:apply-templates select="skills/skillGroup"/>
        </table>
      </body>
    </html>
  </xsl:template>

  <xsl:template name="implantBody">
    <xsl:param name="classType"/>
    <xsl:param name="displayType"/>
    <tr>
      <xsl:element name="td">
        <xsl:attribute name="class">
          implantName <xsl:value-of select="$classType"/>ImplantName
        </xsl:attribute>
        <xsl:value-of select="augmentatorName/child::text()"/>
      </xsl:element>
      <xsl:element name="td">
        <xsl:attribute name="class">
          implantBonus <xsl:value-of select="$classType"/>ImplantBonus
        </xsl:attribute>
        +<xsl:value-of select="augmentatorValue/child::text()"/><xsl:value-of select="' '"/><xsl:value-of select="$displayType"/>
      </xsl:element>
    </tr>
  </xsl:template>

  <xsl:template match="intelligenceBonus">
    <xsl:call-template name="implantBody">
      <xsl:with-param name="classType">intelligence</xsl:with-param>
      <xsl:with-param name="displayType">Intelligence</xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <xsl:template match="charismaBonus">
    <xsl:call-template name="implantBody">
      <xsl:with-param name="classType">charisma</xsl:with-param>
      <xsl:with-param name="displayType">Charisma</xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <xsl:template match="perceptionBonus">
    <xsl:call-template name="implantBody">
      <xsl:with-param name="classType">perception</xsl:with-param>
      <xsl:with-param name="displayType">Perception</xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <xsl:template match="memoryBonus">
    <xsl:call-template name="implantBody">
      <xsl:with-param name="classType">memory</xsl:with-param>
      <xsl:with-param name="displayType">Memory</xsl:with-param>
    </xsl:call-template>
  </xsl:template>
  <xsl:template match="willpowerBonus">
    <xsl:call-template name="implantBody">
      <xsl:with-param name="classType">willpower</xsl:with-param>
      <xsl:with-param name="displayType">Willpower</xsl:with-param>
    </xsl:call-template>
  </xsl:template>

  <xsl:template match="skillGroup">
    <tr class="skillGroupHeader">
      <td colspan="4">
        <xsl:value-of select="@groupName"/> -
        <xsl:value-of select="count(skill)"/> Skill<xsl:if test="count(skill)>1">s</xsl:if>,
        <xsl:value-of select="sum(skill/skillpoints/child::text())"/> Points
      </td>
    </tr>
    <tr class="skillGroupColumnHeaders">
      <td class="spacer"></td>
      <td class="name">Name</td>
      <td class="level">Level</td>
      <td class="skillpoints">Points</td>
    </tr>
    <xsl:apply-templates select="skill"/>
  </xsl:template>

  <xsl:template match="skill">
    <tr class="skillRow">
      <td class="spacer"></td>
      <td class="name">
        <xsl:value-of select="@typeName"/> (Rank <xsl:value-of select="rank/child::text()"/>)
      </td>
      <td class="level">
        Level
        <xsl:choose>
          <xsl:when test="number(level/child::text())=1">I</xsl:when>
          <xsl:when test="number(level/child::text())=2">II</xsl:when>
          <xsl:when test="number(level/child::text())=3">III</xsl:when>
          <xsl:when test="number(level/child::text())=4">IV</xsl:when>
          <xsl:when test="number(level/child::text())=5">V</xsl:when>
        </xsl:choose>
      </td>
      <td class="skillpoints">
        <xsl:value-of select="skillpoints/child::text()"/>/<xsl:value-of select="skilllevel5/child::text()"/> Points
      </td>
    </tr>
    <xsl:if test="ancestor::character/skillInTraining/SkillName/child::text() = @typeName">
      <tr class="skillInTraining">
        <td colspan="4">
          Currently training to level
          <xsl:choose>
            <xsl:when test="number(ancestor::character/skillInTraining/TrainingToLevel/child::text())=1">I</xsl:when>
            <xsl:when test="number(ancestor::character/skillInTraining/TrainingToLevel/child::text())=2">II</xsl:when>
            <xsl:when test="number(ancestor::character/skillInTraining/TrainingToLevel/child::text())=3">III</xsl:when>
            <xsl:when test="number(ancestor::character/skillInTraining/TrainingToLevel/child::text())=4">IV</xsl:when>
            <xsl:when test="number(ancestor::character/skillInTraining/TrainingToLevel/child::text())=5">V</xsl:when>
          </xsl:choose>, will complete <xsl:value-of select="ancestor::character/skillInTraining/EstimatedCompletion/child::text()"/>.
        </td>
      </tr>
    </xsl:if>
  </xsl:template>
</xsl:stylesheet>
